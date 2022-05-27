using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearning.BLL
{
    public static class SVM
    {
        public static MulticlassSupportVectorMachine<Linear> Learn(double[][] inputs, int[] outputs)
        {
            var teacher = new MulticlassSupportVectorLearning<Linear>()
            {
                Learner = (p) => new LinearDualCoordinateDescent()
                {
                    Loss = Loss.L2
                }
            };

            var svm = teacher.Learn(inputs, outputs);

            return svm;
        }
    }
}
