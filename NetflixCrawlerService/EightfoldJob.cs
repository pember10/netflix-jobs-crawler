using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetflixCrawlerService
{
    public class EightfoldJob
    {
        public Position[] positions { get; set; }
    }

    public class Position
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
        public string id_locale { get; set; }
        public string job_description { get; set; }
        public string locale { get; set; }
        public int stars { get; set; }
        public object medallionProgram { get; set; }
        public object location_flexibility { get; set; }
        public string work_location_option { get; set; }
        public string canonicalPositionUrl { get; set; }
        public bool isPrivate { get; set; }
    }

}
