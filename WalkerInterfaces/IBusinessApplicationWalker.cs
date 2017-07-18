using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkerInterfaces
{
    /// <summary>
    /// Interfejs dla biblioteki odpowiedzialnej za testowanie generycznej aplikacji biznesowej,
    /// dostarczający podstawowe metody umożliwiające nawigację po takiej aplikacji
    /// </summary>
    public interface IBusinessApplicationWalker
    {
        /// <summary>
        /// ustawia bazowy url aplikacji
        /// </summary>
        /// <param name="baseUrl"></param>
        void BaseUrl(string baseUrl);

        /// <summary>
        /// logowanie się do aplikacji
        /// </summary>
        void Login(string userName, string password);

        /// <summary>
        /// wylogowanie się z aplikacji
        /// </summary>
        void Logout();

        /// <summary>
        /// pozwala przejść do dowolnej strony w ramach aplikacji
        /// </summary>
        /// <param name="appUrl">adres url wewnątrz aplikacji (nie uwzględnia adresu strony), chyba że rozpoczyna się od http/https</param>
        void Navigate(string appUrl);

        /// <summary>
        /// wyświetla formualrz szczegółów obiektu
        /// </summary>
        void NavigateDetails(string className, string primaryKey);

        /// <summary>
        /// wyświetla formularz listy (master)
        /// </summary>
        /// <param name="className">klasa obiektu listy/nazwa listy</param>
        void NavigateList(string className);

        /// <summary>
        /// wyświetla formularz nowego obiektu
        /// </summary>
        /// <param name="className">klasa obiektu do utworzenia</param>
        /// <returns>klucz główny obiektu</returns>
        string NavigateNew(string className);

        /// <summary>
        /// ustawia wartość we wskazanym polu
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        void Set(string field, string value);

        /// <summary>
        /// odczytuje wartość z pola
        /// </summary>
        /// <param name="field">nazwa pola (przetłumaczona) lub jego #id</param>
        /// <returns>zwrócona wartość</returns>
        string Get(string field);

        /// <summary>
        /// kliknięcie na elemencie
        /// </summary>
        void ClickButton(string label);

        /// <summary>
        /// Zwraca liczbę elementów na liście
        /// </summary>
        /// <param name="list">nazwa listy (etykieta) lub jej #id</param>
        string CountList(string list);

        /// <summary>
        /// Wykonuje kliknięcie na wskazanej liście
        /// </summary>
        /// <param name="list">nazwa listy (etykieta) lub jej #id</param>
        /// <param name="column">nazwa kolumny, na którą mamy kliknąć</param>
        /// <param name="criteria">warunki jakie musi spełniać element na liście by był kliknięty, puste - powoduje kliknięcie pierwszego elementu. Składnia:
        /// kolumna1:wartość1[;kolumna2:wartość2[;...]]</param>
        void ClickList(string list, string column = null, string criteria = null);

        /// <summary>
        /// pobiera wartość z e wskazanej
        /// </summary>
        /// <param name="list">nazwa listy (etykieta) lub jej #id</param>
        /// <param name="column">nazwa kolumny, na którą mamy kliknąć</param>
        /// <param name="criteria">warunki jakie musi spełniać element na liście by był kliknięty, puste - powoduje kliknięcie pierwszego elementu. Składnia:
        /// kolumna1:wartość1[;kolumna2:wartość2[;...]]</param>
        string GetCell(string list, string column, string criteria = null);

        /// <summary>
        /// Wybiera pozycję z menu
        /// </summary>
        /// <param name="menu">pozycja menu, poszczególne pozycje separowane znakiem "/", escape - poprzez podwójny "//"</param>
        /// <example>
        /// &lt;SelectMenu menu="Przejdź do.../Zgłoszenia" /&gt;
        /// </example>
        void SelectMenu(string menu);

        /// <summary>
        /// wybiera (uaktywnia) zakładkę
        /// </summary>
        /// <param name="label"></param>
        void SwitchTab(string label);

        /// <summary>
        /// oczekuje na odblokowanie przeglądarki, komunikację z serwerem, etc (wynikające
        /// z logiki biznesowej aplikacji)
        /// </summary>
        void Wait();

        /// <summary>
        /// zmienia rozmiar okna przeglądarki na ustalony
        /// </summary>
        void BrowserSize(int width, int height);

    }
}
