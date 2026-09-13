# One-time CI/CD setup (GitHub Actions)

Everything the repo owner must do **once** before the pipelines in
`.github/workflows/` can ship a release. The workflows themselves are already
in the repo: `ci.yml` (PR/master gate), `alpha.yml` (master merges → Play
internal track + TestFlight) and `release.yml` (release branches → approval
gate → Google Play + App Store + GitHub Pages + GitHub release/tag).

> ⚠️ **Identifier check before the first release**: the workflows publish to
> `com.kazo0.dexuno` (the `APP_ID` env at the top of `release.yml` and
> `alpha.yml`: Play `packageName`, iOS `bundle-id`, fastlane `app_identifier`).
> The csproj's `ApplicationId` must match it — if you change one, change the
> other. Store identifiers cannot be changed once an app is published.

## 1. Gather the credentials

### Android signing (upload keystore)

This is a new app, so create the upload key (keep the file and its passwords
somewhere safe — Play App Signing lets you reset a lost *upload* key through
Play Console support, but only after the app exists):

```bash
keytool -genkeypair -v -keystore dexuno-upload.keystore -alias dexuno \
        -keyalg RSA -keysize 2048 -validity 10000
```

Note the store password, key alias (`dexuno`) and key password.

### Google Play

1. **Play Console**: create the app (*All apps → Create app*), package name
   `com.kazo0.dexuno`, and complete the store listing / content-rating /
   data-safety questionnaires (the app collects nothing — see
   `docs/privacy-policy.md`, which is also the privacy-policy URL to enter:
   `https://github.com/kazo0/DexUno/blob/master/docs/privacy-policy.md`).
2. Enrol in **Play App Signing** when prompted on the first upload; the keystore
   above becomes the upload key.
3. **The first bundle must be uploaded by hand** through the Console (any
   track, internal is fine): the Play Developer API refuses to publish to an
   app that has never had a bundle uploaded, so `alpha.yml`/`release.yml`
   fail with "package not found" until then. Build it with the commands under
   "Reproducing a store build locally" in `AGENTS.md`, or download the
   `android` artifact from a failed run.
4. **Service account**: in Google Cloud Console (any project): *IAM & Admin →
   Service Accounts → Create service account* (e.g. `github-actions-play`),
   no cloud roles needed. Create a **JSON key** and download it. Then in Play
   Console: *Users and permissions → Invite new users* → the service account's
   email → grant **Release to production** (or scope it to DexUno).

### Apple

1. **App Store Connect**: register the bundle id `com.kazo0.dexuno` at
   developer.apple.com → *Identifiers*, then create the app record in App
   Store Connect (*My Apps → +*). `fastlane deliver` uploads to an existing
   record; it does not create one. Fill in the privacy questionnaire (no data
   collected) and the privacy-policy URL above.
2. **Apple Distribution certificate** exported as `.p12` with its password
   (Keychain Access → export, or create via developer.apple.com → Certificates).
   The Daily Reflection certificate is the same team's and can be reused.
3. An **App Store provisioning profile** for the bundle id must exist at
   developer.apple.com → Profiles (the workflow downloads it fresh by name at
   build time — you don't store the profile itself as a secret).
4. **App Store Connect API key**: App Store Connect → *Users and Access →
   Integrations → App Store Connect API* → generate a key with **App Manager**
   role (or reuse the Daily Reflection one — keys are per team). Save the
   `.p8` file (downloadable once), the **Key ID**, and the **Issuer ID**.

### GitHub Pages (the web app)

Repo **Settings → Pages → Build and deployment → Source: GitHub Actions**.
Nothing else: `release.yml` uploads the published wasm site with
`actions/upload-pages-artifact` and deploys it with `actions/deploy-pages` to
`https://kazo0.github.io/DexUno/`. GitHub creates the `github-pages`
environment on first use with a deployment-branch policy of the default branch
only — releases deploy from `release/**`, so after the first run (or up front
under *Settings → Environments → github-pages*) add `release/**` to its
allowed deployment branches, otherwise the deploy job is refused.

## 2. Set the GitHub secrets and variables

From the repo root (gh CLI, authenticated as `kazo0`), on macOS. Commands
without `--body`/`<` prompt for the value interactively.

```bash
# Android
gh secret set ANDROID_KEYSTORE_BASE64 --body "$(base64 -i ~/path/to/dexuno-upload.keystore)"
gh secret set ANDROID_KEYSTORE_PASSWORD
gh secret set ANDROID_KEY_ALIAS --body dexuno
gh secret set ANDROID_KEY_PASSWORD

# Google Play
gh secret set GOOGLE_PLAY_SERVICE_ACCOUNT_JSON < ~/path/to/service-account.json

# Apple
gh secret set APPLE_CERT_P12_BASE64 --body "$(base64 -i ~/path/to/distribution.p12)"
gh secret set APPLE_CERT_P12_PASSWORD
gh secret set APPSTORE_ISSUER_ID
gh secret set APPSTORE_KEY_ID
gh secret set APPSTORE_PRIVATE_KEY < ~/path/to/AuthKey_XXXXXXXX.p8

# Non-secret variables
gh variable set APPLE_CODESIGN_KEY --body "Apple Distribution: <name> (<TEAMID>)"   # cert common name, exactly as in Keychain
gh variable set APPLE_PROFILE_NAME --body "<App Store provisioning profile name>"
```

`security find-identity -v -p codesigning` prints the certificate common name
to paste into `APPLE_CODESIGN_KEY` verbatim.

The names are the same ones the DailyReflection repo uses, so the Apple
values can be copied across; the Android keystore and the Play service
account are per app. Verify with `gh secret list` / `gh variable list`. The
names must match the `secrets.*` / `vars.*` references in the workflows
exactly — a typo surfaces only when a run reaches the signing step.

## 3. Create the approval gate

Repo **Settings → Environments → New environment** named `production`:

- Add **Required reviewers** → yourself.
- Restrict **deployment branches** to `release/**`, so the publish job can only
  ever run from a release branch.

This is what pauses `release.yml` after the builds and before any store
upload. One approval releases the single publish job (and, after it, the
GitHub Pages deploy).

## 4. Branch rules (the merge gate)

Use **rulesets** (*Settings → Rules → Rulesets*):

- **`master`**: pull request required with **1 approving review**,
  review-thread resolution required, squash/rebase merges only, and the CI
  job names as required status checks — **Formatting**, **Build desktop
  (Skia)**, **Build WebAssembly**, **Build Windows (WinAppSDK)**, **Build
  Android (unsigned)**, **Build iOS (simulator)**. Status checks must exist
  before they can be required, so add them after the first CI run has
  reported them. Keep repository-admin bypass in mind: a bypassed push is
  possible for the owner and is forbidden to agents (see `AGENTS.md`).
- **`release branches`**: block **deletion** and **force-push** on
  `refs/heads/release/**`, with **no bypass actors** — the source of shipped
  builds. Ordinary pushes are unaffected, so the hotfix flow (commit straight
  to `release/v1.0`) still works.

## 5. Local tooling

```bash
dotnet tool install -g nbgv                 # cuts release branches (installed: 3.10.94)
sudo dotnet workload install wasm-tools     # only to reproduce the wasm publish locally
```

## 6. First release (recommended sequence)

1. Complete §1–§4, including the **manual first upload** to Play (§1) and the
   App Store Connect app record.
2. Merge the feature branch to `master` via PR (CI must be green). The merge
   triggers `alpha.yml`: check that a build lands on the Play internal track
   and in TestFlight, and install both — this is the first time the app runs
   under Native AOT on a device (see §7).
3. On master: `nbgv prepare-release` — creates `release/v1.0` with a stable
   version in `version.json` and bumps master to `1.1-alpha`. Push both:
   `git push origin master release/v1.0`.
4. The push triggers **Release**. Let all build jobs finish, but **don't
   approve yet** — instead do a dry run: *Actions → Release → Run workflow*
   on `release/v1.0` with `play_track=internal`, `submit_for_review=false`
   and `deploy_web=false`, then approve that run. This exercises the entire
   pipeline without touching production, App Store review or the live site.
5. Check results: build on the Play **internal** track, build in
   **TestFlight**, GitHub release `v1.0.x` with `.aab`/`.apk`/`.ipa`/desktop
   zips/web zip attached.
6. When happy, **push a follow-up commit** to the release branch and approve
   that run — it publishes Play **production**, submits the iOS build for
   **App Store review** (auto release on approval), deploys the web app to
   GitHub Pages and tags the release.

   Do *not* simply re-run the workflow on the same commit after a dry run.
   The store version numbers are derived from the git height (see "Versioning"
   below), so the same commit always produces the same Android `versionCode`
   and the same iOS `CFBundleVersion` — and both stores reject a re-upload of
   a build number they have already seen, even from a different track. A new
   commit increments the height and sidesteps that.

### Versioning (automatic — nothing to configure)

Version numbers come from Nerdbank.GitVersioning and are computed **in CI, per
build**; no file is hand-edited during a release and the workflow takes no
version input.

- `version.json` at the repo root is the source of truth. `master` carries
  `1.0-alpha`; `nbgv prepare-release` writes the stable `1.0` onto
  `release/v1.0` and bumps master to the next alpha. That command is the one
  manual step, and it runs locally, not in CI.
- `publicReleaseRefSpec` matches `^refs/heads/release/.*$`, so only release
  branches produce clean versions — elsewhere the version carries a
  `-gCOMMITID` suffix, which is what keeps a stray build from looking like a
  shippable one.
- The `version` job in `release.yml` recomputes the version and **fails the
  run** unless `PublicRelease` is `True` and the prerelease tag is empty. A
  branch that was not cut with `nbgv prepare-release` therefore cannot reach
  the stores. Its `SimpleVersion` output names the artifacts, the GitHub
  release, and the `vX.Y.Z` tag.
- The store versions are set by NBGV's own targets during each mobile build —
  `NBGV_SetVersionForMauiAndroid` (before `_GetAndroidPackageName`) and
  `NBGV_SetVersionForMauiIOS` (before `_CompileAppManifest`). They apply to
  this app even though it is not MAUI: the conditions only test
  `TargetPlatformIdentifier`. Android gets
  `versionCode = major<<24 | minor<<16 | height` (1.0.x ⇒ 16777216 + x;
  verified locally: 16777216 / `1.0.0-alpha` on this branch) and iOS gets the
  three-part version for both `CFBundleVersion` and `CFBundleShortVersionString`.
- Every job that builds or computes a version checks out with
  `fetch-depth: 0`; NBGV needs full history to compute the height, and a
  shallow clone would silently change the numbers.

To see what a commit would ship as, run `nbgv get-version` locally, or

```bash
dotnet msbuild DexUno.Modern/DexUno.Modern/DexUno.Modern.csproj -restore -t:_GetAndroidPackageName \
  -p:TargetFramework=net10.0-android -p:TargetFrameworkOverride=android -p:PublicRelease=true \
  -getProperty:ApplicationVersion -getProperty:ApplicationDisplayVersion
```

for the exact store values.

Hotfixes: commit to the same `release/v1.0` branch — each push builds a new
`1.0.<height>` and waits for approval again.

## 7. Native AOT store packages

The `.aab`/`.apk` and `.ipa` that `alpha.yml` and `release.yml` ship are
**Native AOT** (<https://platform.uno/docs/articles/features/native-aot.html>)
— faster startup at the cost of a larger package. Nothing to set up: the
`PublishAot` block in `DexUno.Modern/DexUno.Modern/DexUno.Modern.csproj`
turns it on for `dotnet publish` of the mobile TFMs, the workflows pass the
`PublishNativeAot` switch, and the Android job points the SDK at the runner's
NDK r27.3 (`ANDROID_NDK_HOME`). Desktop zips (CoreCLR, self-contained) and the
wasm site (the .NET WebAssembly SDK's own trimming) are unchanged.

What to know before the first Native AOT release:

- **This app has not been AOT-compiled yet.** The first alpha build (step 2 of
  §6) is the moment to install the internal-track build and the TestFlight
  build on real devices and click through every section — Native AOT removes
  code the trimmer cannot see, and a binding to a property that was trimmed
  shows up as an empty control, not a crash. Everyday CI (PRs into `master`,
  pushes to `master`) does **not** run the AOT compile — it is slow — so the
  AOT build of a change happens in `alpha.yml` after the merge, on the release
  branch itself (push → `release.yml`), or in a PR *into* a `release/*`
  branch, where `ci.yml` switches to the same unsigned AOT publish.
- **Falling back.** Run the workflow manually with `native_aot=false` to ship
  the same commit with the runtime's own Mono AOT instead. Do this rather than
  editing the csproj on the release branch.
- **Android is "experimental" per Microsoft.** The .NET for Android SDK prints
  warning `XA1040` on every Native AOT publish; Uno Platform documents and
  ships it. It only affects the 64-bit ABIs the app already builds
  (`android-arm64;android-x64` are the SDK defaults — no device coverage lost).
- **Reproducing a store build locally** (commands in `AGENTS.md`, "Native AOT
  publish"): iOS needs Xcode and the `ios` workload; Android additionally
  needs an NDK r27+ (`sdkmanager --install "ndk;27.3.13750724"` matches CI).
  Add Uno's diagnostic flags (`-p:TrimmerSingleWarn=false
  -p:_ExtraTrimmerArgs=--verbose -p:IlcGenerateMetadataLog=true
  -p:IlcGenerateMstatFile=true`) to see exactly what was trimmed and which
  members remain reachable by reflection.

## 8. Alpha builds on every master merge

`.github/workflows/alpha.yml` builds a signed Native AOT `.aab` and `.ipa` on
every push to `master` that touches the modern app, and pushes them to the
Google Play **internal** track and **TestFlight**. Docs-only merges and
changes to the legacy app (`DexUno/`, `DexUno.Core/`, `DexUno.sln`) are
skipped (`paths-ignore`), because each upload consumes a store build number.

There is no approval gate: the internal track and TestFlight are the only
destinations, and the iOS build is uploaded with `fastlane pilot` rather than
`deliver`, so it never touches the App Store version record or the review
queue. `release.yml` remains the only path to a public release, and the only
thing that deploys the web app.

One-time setup on the store side:

- **TestFlight**: App Store Connect → *TestFlight → Internal Testing* → add
  yourself to a group with *automatically distribute builds* enabled.
  Otherwise builds arrive and sit there unassigned. Apple processes each
  upload for a few minutes before it appears.
- **Play**: Play Console → *Testing → Internal testing* → add your Google
  account as a tester and accept the opt-in link once. The service account
  already has upload rights from §1.

Things to know:

- **Alpha and release build numbers cannot collide**, so this workflow needs no
  coordination with release branches. Cutting a release branch does not reset
  its height: it continues master's sequence, while master jumps to the next
  minor and restarts there. Since the cut always adds a commit on top of
  wherever master was, a release build is always numbered above every alpha
  build that preceded it. Cut a release branch when you are ready to stabilize
  a version, not before — after the cut, anything merged to master is in the
  *next* version and would have to be cherry-picked to reach the release.
- **Play track ordering.** Once master is developing the next version, the
  internal track carries higher version codes than production does. That is
  the normal state of affairs and Play allows it, but it does mean an internal
  tester stays on the alpha build rather than dropping back to a production
  release.
- **It is slow on purpose.** Native AOT means roughly 10 minutes for Android
  and 25-30 for iOS per merge. A plain build would be far quicker but would not
  exercise the trimmer, which is the whole reason these builds exist.

## 9. The web app

`release.yml`'s `build-wasm` job publishes the `net10.0-browserwasm` head with
`-p:WasmShellWebAppBasePath=/DexUno/` (GitHub Pages serves a project site
under `/<repo>/`; local builds keep `/`), drops a `.nojekyll` marker into the
site (Pages would otherwise run Jekyll and discard the `_framework/` folder the
runtime lives in) and uploads it as the Pages artifact plus a
`DexUno-<version>-web.zip` for the GitHub release. `deploy-web` runs after the
store publish — so after the approval gate — and can be skipped on a dispatch
run with `deploy_web=false`. `Platforms/WebAssembly/manifest.webmanifest`
uses relative `start_url`/`scope` so the PWA manifest is valid at either path.

The Windows head (`net10.0-windows10.0.26100`) is built by CI on
`windows-latest` but is not packaged or published; Microsoft Store (MSIX)
publishing is a follow-up.

## Reference: what lives where

| Thing | Location |
|---|---|
| Version source of truth | `version.json` (repo root, Nerdbank.GitVersioning) |
| versionCode scheme | NBGV built-in: `major<<24 \| minor<<16 \| height` (1.0.x ⇒ 16777216+x) |
| Store version mapping | NBGV targets `NBGV_SetVersionForMauiAndroid` / `NBGV_SetVersionForMauiIOS` (see comment in `DexUno.Modern/Directory.Build.props`) |
| Merge gate | `.github/workflows/ci.yml` + the `master` ruleset (6 required checks, 1 approval) |
| Release-branch guard | The `release branches` ruleset (no deletion, no force-push, no bypass) |
| Release pipeline | `.github/workflows/release.yml` (trigger: push to `release/**`) |
| Alpha pipeline | `.github/workflows/alpha.yml` (trigger: push to `master` touching the modern app) |
| Native AOT switch | `PublishAot` block in `DexUno.Modern/DexUno.Modern/DexUno.Modern.csproj`; per-run override via the `native_aot` dispatch input (or `-p:PublishNativeAot=false` locally) |
| Approval gate | GitHub Environment `production` |
| Web app | GitHub Pages, `github-pages` environment, https://kazo0.github.io/DexUno/ |
| Privacy policy (store listings) | `docs/privacy-policy.md` |
| Secrets/variables | GitHub repo Settings → Secrets and variables → Actions |
