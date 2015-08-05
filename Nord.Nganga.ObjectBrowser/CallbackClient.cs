using System.Collections;
using OBS.Nord.Nganga.ObjectBrowser;

namespace Nord.Nganga.ObjectBrowser
{
  /// <summary>
  /// Summary description for CallbackClient.
  /// </summary>
  class CallbackClient
  {
    private System.Collections.Hashtable CallbackRegistrations;


    public CallbackClient ()
    {
      this.CallbackRegistrations = new System.Collections.Hashtable();
    }

    public void RegisterCallback (object Client, string propertyName)
    {
      lock (this.CallbackRegistrations)
      {
        System.Collections.Hashtable CallbackClients;
        if (this.CallbackRegistrations.ContainsKey(propertyName))
        {
          CallbackClients = (System.Collections.Hashtable) this.CallbackRegistrations[propertyName];
        }
        else
        {
          CallbackClients = new Hashtable();
          this.CallbackRegistrations.Add(propertyName, CallbackClients);
        }
        if (!CallbackClients.ContainsKey(Client.GetHashCode()))
        {
          CallbackClients.Add(Client.GetHashCode(), Client);
        }
      }
    }

    public void DeRegisterCallback (object Client)
    {
      foreach (DictionaryEntry de in this.CallbackRegistrations)
      {
        this.DeRegisterCallback(Client, de.Key.ToString());
      }
    }


    public void DeRegisterCallback (object Client, string propertyName)
    {
      lock (this.CallbackRegistrations)
      {
        System.Collections.Hashtable CallbackClients;
        if (this.CallbackRegistrations.ContainsKey(propertyName))
        {
          CallbackClients = (System.Collections.Hashtable) this.CallbackRegistrations[propertyName];
          if (!CallbackClients.ContainsKey(Client.GetHashCode()))
          {
            CallbackClients.Remove(Client.GetHashCode());
          }
          if (CallbackClients.Count == 0)
          {
            this.CallbackRegistrations.Remove(propertyName);
          }
        }
      }
    }

    public void NotifyCallbacks (string propertyName)
    {
      lock (this.CallbackRegistrations)
      {
        System.Collections.Hashtable CallbackClients;
        if (this.CallbackRegistrations.ContainsKey(propertyName))
        {
          CallbackClients = (System.Collections.Hashtable) this.CallbackRegistrations[propertyName];
          foreach (DictionaryEntry callbackClient in CallbackClients)
          {
            /*
            ((ICallbackClient) callbackClient.Value).Property_Update( 
                propertyName , 
                this.GetPropertyValue( propertyName ) ) ;
                */
            ((ICallbackClient) callbackClient.Value).Property_Update(
                propertyName);
          }
        }
      }
    }
  }
}
