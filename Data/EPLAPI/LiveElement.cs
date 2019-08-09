using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

public class LiveElement
{
    private static Logger _log = LogManager.GetCurrentClassLogger();
    public int id { get; set; }
    public List<Explain> explain {get;set;}
    public LiveElementStats stats {get;set;} = new LiveElementStats();

    public List<FootballerScoreDetailElement> getExplains() {
        var parsed = new List<FootballerScoreDetailElement>();
        FootballerScoreDetailElement parsedExplains = new FootballerScoreDetailElement();
        foreach (var singleExplain in explain) {
            foreach (var stat in singleExplain.stats) {
                var fieldName = stat.identifier;
                var scoreExplain = new ScoreExplain();
                scoreExplain.name = fieldName;
                scoreExplain.points = stat.points;
                scoreExplain.value = stat.value;
                setField(parsedExplains, fieldName, scoreExplain);
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


