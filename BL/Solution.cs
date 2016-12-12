﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface ISolution
    {
        List<double> VariableValues { get; }
        List<double> CriterionValues { get; }
        void SetCriterions(FeaConnector.FeaConnector connector);
    }

    public class Solution: ISolution
    {
        public List<double> VariableValues
        {
            get
            {
                return variableValues;
            }
        }

        public List<double> CriterionValues
        {
            get
            {
                return criterionValues;
            }
        }

        public void SetCriterions(FeaConnector.FeaConnector connector)
        {

        }

        private List<double> variableValues;
        private List<double> criterionValues;
    }
}