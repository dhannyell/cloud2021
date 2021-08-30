﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFunction.Models
{
    public class Acao
    {
        private string _codigo;
        public string Codigo
        {
            get => _codigo;
            set => _codigo = value?.Trim().ToUpper();
        }

        public double? Valor { get; set; }
    }
}
