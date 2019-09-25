using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

public class Fixture
{
    private static Logger _log = LogManager.GetCurrentClassLogger();
    public int id{get;set;}
    
    public string kickoff_time_formatted{get;set;}
    
    public bool started{get;set;}
    
    public List<HomeAwayStats> stats {get; set;}
    
    public EventStats parsedStats{get;set;}
    
    public int code{get;set;}
    
    public string kickoff_time{get;set;}
    
    public int team_h_score {get;set;}
    
    public int team_a_score{get;set;}
    
    public bool finished{get;set;}
    public bool finished_provisional{get;set;}
    
    public int minutes{get;set;}
    
    public bool provisional_start_time{get;set;}

    [JsonProperty(PropertyName="event")]
    public int eventId {get;set;}
    
    public int team_a{get;set;}
    
    public int team_h{get;set;}
}
