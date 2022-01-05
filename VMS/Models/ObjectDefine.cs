using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VMS.Models
{
    public class ObjectDefine
    {
    }

    public class EventSearchRoot
    {
        public EventSearchResult AcsEventSearchResult { get; set; }
    }

    public class EventSearchResult
    {
        public string numOfMatches { get; set; }
        public string totalMatches { get; set; }
        public string responseStatusStrg { get; set; }
        public List<EventInfo> MatchList { get; set; }
    }

    public class EventInfo
    {
        public string major;
        public string minor;
        public string time;
        public DateTime eventdate;
        public string Atime;
        public string ADate;
        public string employeeNoString;
        public string cardNo;
        public string cardReaderNo;
        public string doorNo;
        public string currentVerifyMode;
        public string serialNo;
        public string type;
        public string mask;
        public string name;
        public string userType;
    }


    public class EhomeParams
    {
        public string EhomeID { get; set; }
    }

    public class Device
    {
        public EhomeParams EhomeParams { get; set; }
        public bool activeStatus { get; set; }
        public string devIndex { get; set; }
        public string devMode { get; set; }
        public string devName { get; set; }
        public string devStatus { get; set; }
        public string devType { get; set; }
        public string protocolType { get; set; }
        public int videoChannelNum { get; set; }
    }

    public class MatchList
    {
        public Device Device { get; set; }
    }

    public class DeviceSearchResult
    {
        public List<MatchList> MatchList { get; set; }
        public int numOfMatches { get; set; }
        public int totalMatches { get; set; }
    }

    public class DeviceSearchRoot
    {
        public DeviceSearchResult SearchResult { get; set; }
    }

    public class FaceSearchRoot
    {
        public FaceSearchResult FaceInfoSearch { get; set; }
    }

    public class FaceSearchResult
    {
        public string searchID { get; set; }
        public string numOfMatches { get; set; }
        public string totalMatches { get; set; }
        public string responseStatusStrg { get; set; }
        public List<FaceData> FaceInfo { get; set; }
    }

    public class FaceData
    {
        public string employeeNo { get; set; }
        public string faceURL { get; set; }
    }
}