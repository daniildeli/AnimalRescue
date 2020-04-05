﻿using AnimalRescue.Contracts.BusinessLogic.Models;
using System.Threading.Tasks;

namespace AnimalRescue.Contracts.BusinessLogic.Interfaces
{
    public interface ISequenceService
    {
        Task<SequenceDto> GetCurrentSequenceAsync();
        Task<SequenceDto> GetNextSequenceAsync();
    }
}
