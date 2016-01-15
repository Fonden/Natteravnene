using Mehdime.Entity;
using Ninject;
using NR.Entity;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace NR.Abstract
{
    public class NinjectControllerFactoryScope: DefaultControllerFactory
    {
        private IKernel ninjectKernel;
        //private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly INRRepository _userRepository;

        public NinjectControllerFactoryScope(INRRepository userRepository)
        {
            if (userRepository == null) throw new ArgumentNullException("userRepository");
            _userRepository = userRepository;
            ninjectKernel = new StandardKernel();
            AddBindings();
            
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null) return base.GetControllerInstance(requestContext, controllerType);

            var controller = (IController)ninjectKernel.Get(controllerType);

            if (controller == null)
                return base.GetControllerInstance(requestContext, controllerType);

            return controller;
            
            //return  controllerType == null
            //    ? null
            //    : (IController) ninjectKernel.Get(controllerType);
        }

        private void AddBindings()
        {

            //ninjectKernel.Bind<INRRepository>().ToConstant(new Repository());   
            //ninjectKernel.Bind<INRRepository>().ToConstant(GlobalDbContext);
            ninjectKernel.Bind<INRRepository>().ToConstant(_userRepository);
        }

    }
}