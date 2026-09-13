using System.Runtime.CompilerServices;
using Android.App;
using Android.OS;
using AndroidX.Core.Content;
using AndroidX.Window.Java.Layout;
using AndroidX.Window.Layout;
using DexUno.Modern.Presentation;
using Uno.Devices.Sensors;
using Uno.Foundation.Extensibility;
using Uno.UI;
using Uno.UI.Foldable;
using Windows.Foundation;
using Windows.UI.ViewManagement;

namespace DexUno.Modern.Droid;

/// <summary>
/// The Uno.WinUI.Foldable spanning-rects provider, registered early enough to exist and with its gaps filled:
/// a flat fold counts too, spanning rects are usable for a vertical fold, and posture changes are announced.
/// </summary>
/// <remarks>
/// <para><b>Registration.</b> Uno's XAML generator emits this same registration into <c>App.InitializeComponent</c>, but on
/// Android the <c>App</c> is constructed after <c>MainActivity</c>: <c>BaseActivity</c>'s constructor creates the
/// <c>ApplicationView</c>, which looks the extension up exactly once, finds nothing and caches that result — so the stock
/// provider never comes to exist. A module initializer runs before any code in this assembly, including the activity's
/// constructor. <c>ApiExtensibility.Register</c> keeps the first registration, so the generated one becomes a no-op.</para>
/// <para><b>What counts as spanning.</b> The stock provider only reports spanning while Jetpack says the fold is
/// <c>isSeparating</c> — half-opened, or an occluding hinge — so on a flat foldable the seam falls back to the size split
/// and lands next to the crease instead of on it. Jetpack still reports the fold's bounds when flat, so here a fold in
/// any posture is spanning and the seam stays on the crease.</para>
/// <para><b>Spanning rects.</b> The stock <c>GetSpanningRects</c> compares the fold's Y with
/// <c>ApplicationView.VisibleBounds.Y</c>, which the status bar offsets, so for a vertical fold (book posture) it decides the
/// layout is unknown and returns nothing. This re-implementation splits the window bounds Jetpack reports — the same
/// coordinate space (physical pixels, window-relative) as the fold bounds. <c>TwoPaneView</c> converts them to logical pixels.</para>
/// <para><b>Change notification.</b> The stock provider updates its state on every <c>WindowLayoutInfo</c> but tells nobody, so
/// a posture change never reaches <c>TwoPaneView</c>. A second listener on the same tracker raises
/// <see cref="FoldPosture.Changed"/>; it is registered after the stock one, so it runs after that state is updated.</para>
/// </remarks>
internal sealed class HingeAwareSpanningRects : FoldableApplicationViewSpanningRects, IApplicationViewSpanningRects, INativeDualScreenProvider
{
    [ModuleInitializer]
    internal static void Register() =>
        ApiExtensibility.Register(typeof(IApplicationViewSpanningRects), owner => new HingeAwareSpanningRects(owner));

    private readonly PostureListener _listener = new();
    private WindowInfoTrackerCallbackAdapter? _tracker;

    public HingeAwareSpanningRects(object owner)
        : base(owner)
    {
        // Constructed from BaseActivity's constructor, before the activity is attached, so the stock provider's own
        // activity events are the only thing available here — and those are not part of Uno's public surface. The
        // platform callbacks are, and they fire for the same Start/Stop the stock provider listens to.
        if (Application.Context is Application application)
        {
            application.RegisterActivityLifecycleCallbacks(new Lifecycle(this));
        }
    }

    private void Listen(Activity activity)
    {
        _tracker ??= new WindowInfoTrackerCallbackAdapter(WindowInfoTracker.Companion.GetOrCreate(activity));
        _tracker.AddWindowLayoutInfoListener(activity, ContextCompat.GetMainExecutor(activity)!, _listener);
    }

    private void StopListening() => _tracker?.RemoveWindowLayoutInfoListener(_listener);

    /// <summary>Jetpack reported a fold; its bounds have zero width on a Fold-class device, so don't test IsEmpty.</summary>
    private bool HasFold => FoldBounds is { } fold && (fold.Width() > 0 || fold.Height() > 0);

    bool? INativeDualScreenProvider.IsSpanned => HasFold;

    bool INativeDualScreenProvider.SupportsSpanning => HasFold || SupportsSpanning;

    IReadOnlyList<Rect> IApplicationViewSpanningRects.GetSpanningRects()
    {
        if (!HasFold || FoldBounds is not { } fold || ContextHelper.Current is not Activity activity)
        {
            return [];
        }

        var window = WindowMetricsCalculator.Companion.OrCreate.ComputeCurrentWindowMetrics(activity).Bounds;
        var width = window.Width();
        var height = window.Height();

        return fold.Height() >= fold.Width()
            ? [new Rect(0, 0, fold.Left, height), new Rect(fold.Right, 0, width - fold.Right, height)]
            : [new Rect(0, 0, width, fold.Top), new Rect(0, fold.Bottom, width, height - fold.Bottom)];
    }

    private sealed class PostureListener : Java.Lang.Object, AndroidX.Core.Util.IConsumer
    {
        public void Accept(Java.Lang.Object? layoutInfo) => FoldPosture.NotifyChanged();
    }

    /// <summary>Follows the Uno activity only; any other activity (a picker, a share sheet) is ignored.</summary>
    private sealed class Lifecycle(HingeAwareSpanningRects owner) : Java.Lang.Object, Application.IActivityLifecycleCallbacks
    {
        public void OnActivityStarted(Activity activity)
        {
            if (activity is BaseActivity)
            {
                owner.Listen(activity);
            }
        }

        public void OnActivityStopped(Activity activity)
        {
            if (activity is BaseActivity)
            {
                owner.StopListening();
            }
        }

        public void OnActivityCreated(Activity activity, Bundle? savedInstanceState) { }
        public void OnActivityResumed(Activity activity) { }
        public void OnActivityPaused(Activity activity) { }
        public void OnActivitySaveInstanceState(Activity activity, Bundle outState) { }
        public void OnActivityDestroyed(Activity activity) { }
    }
}
