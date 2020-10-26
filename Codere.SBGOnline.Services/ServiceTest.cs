namespace Codere.SBGOnline.Services
{
    #region Using

    using Codere.SBGOnline.Common.DataAccess.Interfaces.Dynamics.Repositories;
    using Codere.SBGOnline.DynamicsClasses;
    using Codere.SBGOnline.Services.Interfaces;
    using System;
    using System.Linq;

    #endregion

    public class ServiceTest : IServiceTest
    {
        private readonly IDynamicsRepository<Contact> _contactsRepository;

        public ServiceTest(IDynamicsRepository<Contact> contactsRepository)
        {
            _contactsRepository = contactsRepository ??
                throw new ArgumentNullException(nameof(contactsRepository));
        }

        public Contact Contacts(string name)
        {
            return _contactsRepository.FindByField("cdr_login", name)?.ToList().FirstOrDefault();
        }
    }
}
