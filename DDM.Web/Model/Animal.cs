using System;

namespace DDM.Web.Model
{
    public class Animal
    {
        public int Id { get; set; }

        public DateTime BirthDate { get; set; }

        public int? BreedId { get; set; }

        public DateTime ExitDate { get; set; }

        public string Name { get; set; }

        public int Number { get; set; }

        public string OfficialRegNo { get; set; }

        public int LactationNumber { get; set; }

        public int? ReproductionStatusId { get; set; }

        public string ReproductionStatusName { get; set; }

        public int? GroupId { get; set; }
    }
}