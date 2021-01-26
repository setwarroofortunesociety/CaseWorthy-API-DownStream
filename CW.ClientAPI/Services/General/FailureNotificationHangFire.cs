using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Logging;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using Microsoft.Extensions.Logging;
 

namespace CW.ClientAPI.Services.General
{
    public class FailureNotificationHangFire : JobFilterAttribute, IClientFilter, 
            IServerFilter, IElectStateFilter, IApplyStateFilter
    {
        private static  ILogger Logger { get; set; }

        public void OnCreating(CreatingContext context)
        {
            Logger.LogInformation(
                String.Format("Creating a job based on method `{0}`...", 
                    context.Job.Method.Name));
        }

        public void OnCreated(CreatedContext context)
        {
            Logger.LogInformation(
                String.Format("Job that is based on method `{0}` has been created with id `{1}`",
                context.Job.Method.Name,
                context.BackgroundJob?.Id));
        }

        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            Logger.LogInformation(
                    String.Format("Background job {0} state was changed from {1}  to {2}.",
                            context.BackgroundJob.Id,
                            context.OldStateName,
                            context.NewState.Name));
             
        }

        public void OnPerforming(PerformingContext context)
        {
            Logger.LogInformation(
                String.Format("Starting to perform job `{0}`", 
                    context.BackgroundJob.Id));
        }

        public void OnPerformed(PerformedContext context)
        {
            Logger.LogInformation(String.Format(CultureInfo.CurrentCulture,"Job '{0}' has been performed", context.BackgroundJob.Id));
        }

        public void OnStateElection(ElectStateContext context)
        {
            var failedState = (FailedState)context.CandidateState;
            if (failedState != null)
            {
                Logger.LogWarning(
                    String.Format("Job `{0}` has been failed due to an exception `{1}`",
                    context.BackgroundJob.Id,
                    failedState.Exception));
            }
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            Logger.LogInformation(
                 String.Format("Background Job {0} state {1} was UnApplied.",
                context.BackgroundJob.Id,
                context.OldStateName));
        }

    }
}
