<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebForms._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <main class="gap-6 p-6 flex flex-1 flex-col">
        <section class="gap-2 flex flex-col items-start" aria-labelledby="aspnetTitle">
            <h1 id="aspnetTitle" class="text-slate-950 text-4xl font-bold">ASP.NET</h1>
            <p class="">ASP.NET is a free web framework for building great Web sites and Web applications using HTML, CSS, and JavaScript.</p>

            <a href="http://www.asp.net" class="py-2 px-4 bg-blue-500 text-white rounded transition-all hover:bg-blue-500/80">Learn more &raquo;
            </a>
        </section>

        <div class="grid-col-1 gap-4 grid md:grid-col-3">
            <section class="gap-2 flex flex-col items-start" aria-labelledby="gettingStartedTitle">
                <h2 id="gettingStartedTitle" class="text-slate-950 text-2xl font-medium">Getting started</h2>

                <p>ASP.NET Web Forms lets you build dynamic websites using a familiar drag-and-drop, event-driven model.
                A design surface and hundreds of controls and components let you rapidly build sophisticated, powerful UI-driven sites with data access.</p>
                <a href="https://go.microsoft.com/fwlink/?LinkId=301948" class="py-2 px-4 bg-blue-500 text-white rounded-md transition-all hover:bg-blue-500/80">Learn more &raquo;</a>
            </section>

            <section class="gap-2 flex flex-col items-start" aria-labelledby="librariesTitle">
                <h2 id="librariesTitle" class="text-slate-950 text-2xl font-medium">Get more libraries</h2>

                <p>NuGet is a free Visual Studio extension that makes it easy to add, remove, and update libraries and tools in Visual Studio projects.</p>
                <a href="https://go.microsoft.com/fwlink/?LinkId=301949" class="py-2 px-4 bg-blue-500 text-white rounded-md transition-all hover:bg-blue-500/80">Learn more &raquo;</a>
            </section>

            <section class="gap-2 flex flex-col items-start" aria-labelledby="hostingTitle">
                <h2 id="hostingTitle" class="text-slate-950 text-2xl font-medium">Web Hosting</h2>

                <p>You can easily find a web hosting company that offers the right mix of features and price for your applications.</p>
                <a href="https://go.microsoft.com/fwlink/?LinkId=301950" class="py-2 px-4 bg-blue-500 text-white rounded-md transition-all hover:bg-blue-500/80">Learn more &raquo;</a>
            </section>
        </div>
    </main>
</asp:Content>
