using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetflixCrawlerService
{

    public class EightfoldPosition
    {
        public long id { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public string[] locations { get; set; }
        public int hot { get; set; }
        public string department { get; set; }
        public string business_unit { get; set; }
        public int t_update { get; set; }
        public int t_create { get; set; }
        public string ats_job_id { get; set; }
        public string display_job_id { get; set; }
        public string type { get; set; }
        public string job_description { get; set; }
        public string id_locale { get; set; }
        public string locale { get; set; }
        public int stars { get; set; }
        public object medallionProgram { get; set; }
        public object location_flexibility { get; set; }
        public object work_location_option { get; set; }
        public string canonicalPositionUrl { get; set; }
        public bool isPrivate { get; set; }
        public CustomJobDetails custom_JD { get; set; }
        public bool hideUploadResumeOption { get; set; }
    }

    public class CustomJobDetails
    {
        public DataFields data_fields { get; set; }
        public Display[] display { get; set; }
        public bool enable { get; set; }
    }

    public class DataFields
    {
        public string[] job_req_id { get; set; }
        public string[] team { get; set; }
        public string[] posting_date { get; set; }
        public string[] work_type { get; set; }
    }

    public class Display
    {
        public string label { get; set; }
        public string value { get; set; }
    }

}
