using System.Threading.Tasks;

public interface IAlertSender
{
    Task SendAlert(int teamId, string title, string subtitle);
}
