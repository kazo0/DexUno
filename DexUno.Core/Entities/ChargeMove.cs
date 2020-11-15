namespace Dex.Core.Entities
{
    public class ChargeMove : Move
    {
        public ChargeMove()
        {
            MoveType = MoveType.Charge;
        }

        public float Critical { get; set; }
        public float Dodge { get; set; }
        public ushort EnergyBars { get; set; }
    }
}