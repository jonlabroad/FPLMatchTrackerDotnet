using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

public class LiveElement
{
    private static Logger _log = LogManager.GetCurrentClassLogger();
    public JArray explain {get;set;}
    public LiveElementStats stats {get;set;}

    public List<FootballerScoreDetailElement> getExplains() {
        var parsed = new List<FootballerScoreDetailElement>();
        for (int i = 0; i < explain.Count; i++) {
            var explainsArray = (JArray) explain[i];
            FootballerScoreDetailElement parsedExplains = new FootballerScoreDetailElement();
            for (var j = 0; j < explainsArray.Count - 1; j++) {
                JObject explainJson = explainsArray[j] as JObject;
                foreach (var prop in explainJson.Properties()) {
                    var fieldName = prop.Name;
                    String elementJson = prop.Value.ToString();
                    ScoreExplain parsedExplain = JsonConvert.DeserializeObject<ScoreExplain>(elementJson);
                    setField(parsedExplains, fieldName, parsedExplain);
                }
            }
            parsed.Add(parsedExplains);
        }
        return parsed;
    }

    private void setField(FootballerScoreDetailElement element, String fieldName, ScoreExplain explain)
    {
        try {
            var field = typeof(FootballerScoreDetailElement).GetProperty(fieldName);
            field.SetValue(element, explain);
        } catch (Exception e) {
            _log.Error(e);
        }
    }
}


