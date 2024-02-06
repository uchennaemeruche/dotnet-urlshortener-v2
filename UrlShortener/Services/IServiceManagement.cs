using System;
namespace UrlShortener.Services
{
	public interface IServiceManagement
	{
		void SendEmail();
		void UpdateDatabase();
		void GenerateUrlCode();
		void SyncRecords();
	}
}

