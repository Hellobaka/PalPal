namespace me.cqp.luohuaming.PalPal.PublicInfos.Models
{
    public class PlayerList
    {
        public PlayerInfo[] players { get; set; }
    }

    public class PlayerInfo
    {
        public string name { get; set; }
        public string accountName { get; set; }
        public string playerId { get; set; }
        public string userId { get; set; }
        public string ip { get; set; }
        public double ping { get; set; }
        public double location_x { get; set; }
        public double location_y { get; set; }
        public int level { get; set; }
    }
}