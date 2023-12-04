using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class IFSC_Data
{
    string _MICR = "";
    public string MICR
    {
        get { return _MICR; }
        set { _MICR = value; }
    }

    string _BRANCH = string.Empty;
    public string BRANCH
    {
        get { return _BRANCH; }
        set { _BRANCH = value; }
    }

    string _ADDRESS = string.Empty;
    public string ADDRESS
    {
        get { return _ADDRESS; }
        set { _ADDRESS = value; }
    }

    string _STATE = string.Empty;
    public string STATE
    {
        get { return _STATE; }
        set { _STATE = value; }
    }

    string _CONTACT = string.Empty;
    public string CONTACT
    {
        get { return _CONTACT; }
        set { _CONTACT = value; }
    }

    string _CITY = string.Empty;
    public string CITY
    {
        get { return _CITY; }
        set { _CITY = value; }
    }

    string _BANK = string.Empty;
    public string BANK
    {
        get { return _BANK; }
        set { _BANK = value; }
    }

    string _BANKCODE = string.Empty;
    public string BANKCODE
    {
        get { return _BANKCODE; }
        set { _BANKCODE = value; }
    }

    string _IFSC = string.Empty;
    public string IFSC
    {
        get { return _IFSC; }
        set { _IFSC = value; }
    }


}
