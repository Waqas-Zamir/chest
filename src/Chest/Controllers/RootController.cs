﻿// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.WebApi
{
    using System.Diagnostics;
    using System.Net;
    using System.Reflection;
    using Chest;
    using Chest.Models;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.SwaggerGen;

    [Route("status")]
    public class RootController : Controller
    {
        private static readonly RootModel Root =
            new RootModel
            {
                Title = typeof(Program).Assembly.Attribute<AssemblyTitleAttribute>(attribute => attribute.Title),
                Version = typeof(Program).Assembly.Attribute<AssemblyInformationalVersionAttribute>(attribute => attribute.InformationalVersion),
                OS = System.Runtime.InteropServices.RuntimeInformation.OSDescription.TrimEnd(),
                ProcessId = Process.GetCurrentProcess().Id,
            };

        [HttpGet]
        [SwaggerOperation("Root_GetStatus")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(RootModel))]
        public IActionResult Get() => this.Ok(Root);
    }
}
