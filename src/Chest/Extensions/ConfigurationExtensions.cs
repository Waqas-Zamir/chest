﻿// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

// TODO (Cameron): Remove suppression (below) when StyleCop supports C# 7.0 tuples.
#pragma warning disable SA1008, SA1009

namespace Chest
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Serilog;

    public static class ConfigurationExtensions
    {
        /*  NOTE (Cameron): This is a little messy but basically the idea here is that the secret values come
            - first from secrets.json (typically on a developers machine); then
            - second from an environment variable (typically inside of a docker environment).
            If a value is not supplied and there is no default specified (below) then an exception will be thrown.  */

        private static readonly List<(string, string, string, string)> EnvironmentSecretConfig = new List<(string, string, string, string)>
        {
            /* secrets.json Key         // Environment Variable     // command line argument    // default value (optional) */
            // Uncomment this with task MTC-20 implement persistence layer
             ("ConnectionStrings:Chest", "CHEST_CONNECTIONSTRING",   "connectionString",         null),
        };

        private static List<Action<ILogger>> logMessages = new List<Action<ILogger>>();

        public static IConfigurationBuilder AddEnvironmentSecrets(this IConfigurationBuilder builder, string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Startup>()
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var secrets = new Dictionary<string, string>();

            foreach (var (key, environmentVariable, commandLineArg, defaultValue) in EnvironmentSecretConfig)
            {
                secrets[key] = configuration[commandLineArg];
                if (secrets[key] != null)
                {
                    logMessages.Add(log => log.Debug("Value of {secretKey} satisfied by command line argument", commandLineArg));
                    continue;
                }

                secrets[key] = configuration[key];
                if (secrets[key] != null)
                {
                    logMessages.Add(log => log.Debug("Value of {secretKey} satisfied by secrets.json", key));
                    continue;
                }

                secrets[key] = configuration[environmentVariable];
                if (secrets[key] != null)
                {
                    logMessages.Add(log => log.Debug("Value of {secretKey} satisfied by environment variable", key));
                    continue;
                }

                secrets[key] = defaultValue;
                if (secrets[key] != null)
                {
                    logMessages.Add(log => log.Warning("Value of {secretKey} satisfied by default value", key));
                }
                else
                {
                    logMessages.Add(
                        log =>
                        log.Error(
                            "Cannot find a value in secrets.json for {secretKey} or a value for the environment variable named {environmentVariable} and no default value was specified.",
                            key,
                            environmentVariable));
                }
            }

            return builder.AddInMemoryCollection(secrets);
        }

        public static void ValidateEnvironmentSecrets(this IConfigurationRoot configuration, ILogger log)
        {
            logMessages.ForEach(logMessage => logMessage(log));

            foreach (var (key, environmentVariable, commandLineArg, defaultValue) in EnvironmentSecretConfig)
            {
                if (configuration.GetValue<string>(key) == null)
                {
                    throw new InvalidOperationException("Validation of environment secrets failed.");
                }
            }
        }

        public static void ValidateConnectionString(this IConfigurationRoot configuration, string connectionStringName)
        {
            var connStr = configuration.GetConnectionString(connectionStringName);

            var url = string.Empty;

            try
            {
                using (var connection = new SqlConnection(connStr))
                {
                    url = connection.DataSource;
                    connection.Open();
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"No connection could be made to database '{connectionStringName}' on '{url}'", e);
            }
        }
    }
}
