using System;
using System.Collections.Generic;

namespace Advisor.DependencyInjection
{
	// TODO: Re-implement IServiceProvider once it is whitelisted. If ever.
	public class AdvisorServiceContainer //: IServiceProvider
	{
		private Dictionary<Type, object> _services;
		
		/// <summary>
		/// The core addon class of Advisor.
		/// </summary>
		public AdvisorCore Advisor { get; }

		public AdvisorServiceContainer(AdvisorCore core)
		{
			_services = new Dictionary<Type, object>();
			Advisor = core;
		}
		
		/// <summary>
		/// Add a service to the container.
		/// </summary>
		/// <param name="service"> The service instance to add. </param>
		/// <typeparam name="T"> The type of the service to add. </typeparam>
		/// <exception cref="InvalidOperationException"> Thrown when a service with the given type already exists. </exception>
		/// <exception cref="ArgumentNullException"> Thrown when the service is null. </exception>
		public void AddService<T>( T service )
		{
			var serviceType = typeof(T);
			if (_services.ContainsKey( serviceType ))
			{
				throw new InvalidOperationException(
					$"Service container already contains a service of type '{serviceType.FullName}'" );
			}

			if (service == null)
			{
				throw new ArgumentNullException(nameof(service));
			}
			
			_services.Add(serviceType, service);
		}
		
		/// <summary>
		/// Get a service by type.
		/// </summary>
		/// <param name="serviceType"> The type of the service to get. </param>
		/// <returns> The service if it exists, else null. </returns>
		public object? GetService(Type serviceType)
		{
			return _services.ContainsKey(serviceType) ? _services[serviceType] : null;
		}

		/// <summary>
		/// Get a service by type, generic edition.
		/// </summary>
		/// <typeparam name="T"> The type of the service to get. </typeparam>
		/// <returns> The service if it exists, else null. </returns>
		public T GetService<T>() where T : class
		{
			return GetService( typeof(T) ) as T;
		}
	}
}
