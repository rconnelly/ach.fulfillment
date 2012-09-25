

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Common.Logging;
using Quartz;
using Quartz.Spi;
using Quartz.Util;

namespace Ach.Fulfillment.Scheduler
{
    public class AchJobFactory : IJobFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AchJobFactory));

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            IJobDetail jobDetail = bundle.JobDetail;
            Type jobType = jobDetail.JobType;
            try
            {
                if (Log.IsDebugEnabled)
                {
                    Log.Debug(string.Format(CultureInfo.InvariantCulture, "Producing instance of Job '{0}', class={1}", jobDetail.Key, jobType.FullName));
                }

                return ObjectUtils.InstantiateType<IJob>(jobType);
            }
            catch (Exception e)
            {
                SchedulerException se = new SchedulerException(string.Format(CultureInfo.InvariantCulture, "Problem instantiating class '{0}'", jobDetail.JobType.FullName), e);
                throw se;
            }
        }
    }
}
