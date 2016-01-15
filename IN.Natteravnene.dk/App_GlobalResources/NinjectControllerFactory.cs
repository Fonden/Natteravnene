using Ninject;
using NR.Entity;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace NR.Abstract
{
    public class NinjectControllerFactory: DefaultControllerFactory
    {
        private IKernel ninjectKernel;

        public NinjectControllerFactory()
        {
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
            ninjectKernel.Bind<INRRepository>().ToConstant(new Repository());
        }

    }
}