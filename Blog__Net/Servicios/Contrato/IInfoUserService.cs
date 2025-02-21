﻿using Microsoft.EntityFrameworkCore;
using Blog__Net.Models;

namespace Blog__Net.Servicios.Contrato
{
    public interface IInfoUserService
    {
        Task<InfoUser> GetInfoUser(string Email, string Passcode);
        Task<InfoUser> SaveInfoUser(InfoUser modelo);
        Task<bool> UserExists(string userName, string email);
        Task<string> GetRoleNameById(int rolId);
    }
}