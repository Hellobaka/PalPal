namespace me.cqp.luohuaming.PalPal.PublicInfos.Models
{
    public class ServerMetrics
    {
        public int serverfps { get; set; } = -1;
        public int currentplayernum { get; set; }
        public double serverframetime { get; set; }
        public int maxplayernum { get; set; }
        public int uptime { get; set; }
    }
}