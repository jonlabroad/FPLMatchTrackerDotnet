public class Event
{
    public int id {get;set;}
    public string name {get;set;}
    public string deadline_time {get;set;}
    public int average_entry_score {get;set;}
    public bool finished {get;set;}
    public bool data_checked {get;set;}
    public int highest_scoring_entry {get;set;}
    public int deadline_time_epoch {get;set;}
    public int deadline_time_game_offset {get;set;}
    public string deadline_time_formatted {get;set;}
    public int highest_score {get;set;}
    public bool is_previous {get;set;}
    public bool is_current {get;set;}
    public bool is_next {get;set;}
}
