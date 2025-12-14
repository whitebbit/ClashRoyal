using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace _ClashRoyal.Scripts.Network
{
    public class Authorization : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        #endregion

        #region FIELDS

        private string _login;
        private string _password;

        #endregion

        #region UNITY FUNCTIONS

        #endregion

        #region METHODS

        public void SetLogin(string login)
        {
            _login = login;
        }

        public void SetPassword(string password)
        {
            _password = password;
        }

        public async void SingIn()
        {
            if (string.IsNullOrEmpty(_login) || string.IsNullOrEmpty(_password))
            {
                OnFailure("Логин или пароль не могут быть пустыми");
                return;
            }

            await NetworkExtensions.PostFormAsync(NetworkExtensions.AuthorizationURL, new Dictionary<string, string>
            {
                { "login", _login },
                { "password", _password }
            }, OnSuccess, OnFailure);
        }

        private void OnFailure(string obj)
        {
            Debug.LogError(obj);
        }

        private void OnSuccess(string data)
        {
            var result = data.Split("|");

            if (result.Length < 2 || result[0] != "ok")
            {
                OnFailure($"Невалидный ответ сервера: {data}");
                return;
            }

            if (int.TryParse(result[1], out var id))
            {
                UserInfo.ID = id;
                Debug.Log(UserInfo.ID);
            }
            else
                OnFailure($"Не удалось преобразовать \"{result[1]}\" в Int. Все данные: {data}");
        }

        #endregion
    }
}