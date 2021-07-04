using System;

enum GuessOption
{
    Letter = 1,
    WholeWord = 2,
    InvalidValue = 3,
}

public class DataRecord {
    string username_;
    string date_;
    long guessing_time;
    string capital_;

    public DataRecord(String username, String date, long time, string capital)
    {
        this.username_ = username;
        this.date_ = date;
        this.guessing_time = time;
        this.capital_ = capital;
    }

    public String getUsername()
    {
        return username_;
    }

    public String getDate()
    {
        return date_;
    }

    public long getTime()
    {
        return guessing_time;
    }

    public String getCapital()
    {
        return capital_;
    }
}