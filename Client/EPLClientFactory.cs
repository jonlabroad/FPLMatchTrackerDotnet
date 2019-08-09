using System;
using NLog;

public class EPLClientFactory {
    private static Logger _log = LogManager.GetCurrentClassLogger();
    public static EPLClient createClient() {
        return createHttpClient();
    }

    public static EPLClient createHttpClient() {
        try {
            return new EPLClient(new RequestExecutor(GlobalConfig.EplBaseUrl, false, 0));
        } catch (Exception e) {
            _log.Error(e);
        }
        return null;
    }

    public static EPLClient createHttpClient(bool record, int currentSequence) {
        try {
            return new EPLClient(new RequestExecutor(GlobalConfig.EplBaseUrl, record, currentSequence));
        } catch (Exception e) {
            _log.Error(e);
        }
        return null;
    }
}
