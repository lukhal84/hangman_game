using System;
public class CountriesCapitals {
    string country_;
    string capital_;

    public CountriesCapitals(String country, String capital)
    {
        this.country_ = country;
        this.capital_ = capital;
    }

    public String getCountry()
    {
        return country_;
    }

    public String getCapital()
    {
        return capital_;
    }
}