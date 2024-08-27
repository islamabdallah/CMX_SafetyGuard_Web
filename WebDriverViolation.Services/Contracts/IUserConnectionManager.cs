using System;
using System.Collections.Generic;
using System.Text;

namespace WebDriverViolation.Services.Contracts
{
    public interface IUserConnectionManager
    {
        void KeepUserConnection(string userId, string connectionId);
        void RemoveUserConnection(string connectionId);
        List<string> GetUserConnections(string userId);
    }
}
