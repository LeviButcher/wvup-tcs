using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.Helpers;
using tcs_service.Models;
using tcs_service.Repos.Interfaces;
using tcs_service.UnitOfWorks.Interfaces;

public class UnitOfWorkSession : IUnitOfWorkSession
{
    private readonly IPersonRepo personRepo;
    private readonly IReasonRepo reasonRepo;
    private readonly ISessionRepo sessionRepo;
    private readonly IClassRepo classRepo;

    public UnitOfWorkSession(IPersonRepo personRepo, IReasonRepo reasonRepo, ISessionRepo sessionRepo, IClassRepo classRepo)
    {
        this.personRepo = personRepo;
        this.reasonRepo = reasonRepo;
        this.sessionRepo = sessionRepo;
        this.classRepo = classRepo;
    }

    public async Task<int> UploadSessions(IEnumerable<CSVSessionUpload> sessionUploads)
    {
        var errorList = new List<string>();
        var personExistsErrors = (await Task.WhenAll(sessionUploads.Select(async x =>
        {
            var personExists = await personRepo.Exist(p => p.Email == x.Email);
            if (!personExists)
                return $"{x.Email} does not exist";
            return null;
        }))).Where(x => x != null);

        var classExistsErrors = (await Task.WhenAll(sessionUploads.SelectMany(x => x.CRNs).
        Select(async x =>
        {
            var classExists = await classRepo.Exist(c => c.CRN == x);
            if (!classExists)
                return $"Class with CRN: {x} does not exist";
            return null;
        }))).Where(x => x != null);

        var reasonExistsErrors = (await Task.WhenAll(sessionUploads.SelectMany(x => x.Reasons).
        Select(async x =>
        {
            var reasonExists = await reasonRepo.Exist(c => c.Name == x);
            if (!reasonExists)
                return $"Reason with Name: '{x}' does not exist";
            return null;
        }))).Where(x => x != null);

        errorList.AddRange(personExistsErrors);
        errorList.AddRange(classExistsErrors);
        errorList.AddRange(reasonExistsErrors);

        if (errorList.Count() != 0)
        {
            throw new TCSException(String.Join("\n", errorList));
        }


        var sessionList = await Task.WhenAll(sessionUploads.Select(async x => new Session()
        {
            Tutoring = x.Tutoring,
            InTime = x.InTime,
            OutTime = x.OutTime,
            PersonId = (await personRepo.Find(p => p.Email == x.Email)).Id,
            SemesterCode = x.SemesterCode,
            SessionClasses = x.CRNs
                  .Select(c => new SessionClass() { ClassId = c }).ToList(),
            SessionReasons = await Task.WhenAll(x.Reasons
                      .Select(s => s.Trim())
                      .Select(async r =>
                      {
                          var reason = await reasonRepo.Find(re => re.Name == r);
                          return new SessionReason()
                          {
                              ReasonId = reason.Id
                          };
                      }))
        }));

        return await sessionRepo.Create(sessionList);
    }
}