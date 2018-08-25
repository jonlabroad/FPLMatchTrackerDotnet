public class EndpointUserData
{
    public string uniqueUserId;
    public string firebaseId;

    public EndpointUserData(string raw) {
        var data = raw.Split(":");
        uniqueUserId = data[0];
        firebaseId = data[1];
    }

    public override string ToString() {
        return string.Format("{0}:{1}", uniqueUserId, firebaseId);
    }
}
