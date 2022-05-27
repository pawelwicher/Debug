using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DDM.Web.Model;
using DDM.Web.Providers;

namespace DDM.Web.Pages
{
    public class IndexModel : PageModel
    {
        private IDataProvider DataProvider { get; }

        public string FarmName { get; set; }

        public IndexModel(IDataProvider dataProvider)
        {
            DataProvider = dataProvider;
        }

        public void OnGet()
        {
            FarmName = DataProvider.GetData<Farm>(@"/farm/1").Name;
        }

        public ActionResult OnGetReproductionStatus()
        {
            var reproductionStatuses = DataProvider
                .GetData<List<ReproductionStatus>>(@"/reproduction/statuses")
                .ToDictionary(x => x.Id, x => x.Name);

            var animals = DataProvider.GetData<List<Animal>>(@"/cows");

            var result = animals
                .Where(x => x.ReproductionStatusId.HasValue && reproductionStatuses[x.ReproductionStatusId.Value] != "Culled")
                .GroupBy(x => x.ReproductionStatusId)
                .Select(x => new Count
                {
                    Name = reproductionStatuses[x.Key.Value],
                    Value = x.Count()
                })
                .ToList();

            return new JsonResult(result);
        }

        public ActionResult OnGetGroupsSummary()
        {
            var animals = DataProvider.GetData<List<Animal>>(@"/cows");
            var groups = DataProvider.GetData<List<Group>>(@"/groups")
                .ToDictionary(x => x.Id);

            var result = animals
               .Where(x => x.GroupId.HasValue)
               .Select(x => new
               {
                   AnimalId = x.Id,
                   GroupId = x.GroupId.Value,
                   HerdId = groups[x.GroupId.Value].HerdId,
               })
               .GroupBy(x => new { x.HerdId, x.GroupId })
               .OrderBy(x => x.Key.HerdId)
               .Select(x => new Count
               {
                   Name = groups[x.Key.GroupId].Name,
                   Value = x.Count()
               });

            return new JsonResult(result);
        }

        public ActionResult OnGetDemographyKpi()
        {
            List<DemographyKpi> result = new List<DemographyKpi>();

            var animals = DataProvider.GetData<List<Animal>>(@"/cows");

            var born = animals
                .GroupBy(x => new DateTime(x.BirthDate.Year, x.BirthDate.Month, 1))
                .ToDictionary(x => x.Key, x => x.Count());

            var exited = animals
                .Where(x => x.ExitDate >= DateTime.MinValue)
                .GroupBy(x => new DateTime(x.ExitDate.Year, x.ExitDate.Month, 1))
                .ToDictionary(x => x.Key, x => x.Count());

            foreach (var key in born.Keys.Union(exited.Keys).OrderBy(x => x))
            {
                result.Add(new DemographyKpi
                {
                    Period = key.Month + "." + key.Year,
                    BornCount = born.ContainsKey(key) ? born[key] : 0,
                    ExitedCount = exited.ContainsKey(key) ? exited[key] : 0,
                });
            }

            if (result.Count > 12)
            {
                return new JsonResult(result.Skip(result.Count - 12));
            }

            return new JsonResult(result);
        }
    }
}