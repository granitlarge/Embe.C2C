namespace Embe.C2C.Application;

public static class Emails
{
    public static string Template(string applicationName, string heading, string htmlBody) =>
    $$"""
        <!DOCTYPE html>
        <html> 
            <head>
                <link rel="preconnect" href="https://fonts.googleapis.com">
                <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
                <link
                    href="https://fonts.googleapis.com/css2?family=Cal+Sans&family=Lato:ital,wght@0,100;0,300;0,400;0,700;0,900;1,100;1,300;1,400;1,700;1,900&family=Roboto:ital,wght@0,100..900;1,100..900&display=swap"
                    rel="stylesheet">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
            </head>
            <style>
                * {
                    box-sizing: border-box;
                    margin: 0;
                    padding: 0;
                }

                .body {
                    background-color: rgb(143, 151, 240);
                    font-family: 'Roboto', sans-serif;
                }

                .container {
                    display: flex;
                    flex-direction: column;
                    justify-items: center;
                    align-items: center;
                    justify-content: center;
                    gap: 1rem;
                    padding-top: 2rem;
                    padding-bottom: 2rem;
                    padding-left: .5rem;
                    padding-right: .5rem;
                    width: 500px;
                    max-width: 100%;
                    margin-inline: auto;
                }

                .surface {
                    background-color: white;
                    border-radius: .75rem;
                    padding: .3rem;
                    display: flex;
                    flex-direction: column;
                    width: 100%;
                    gap: .3rem;
                    align-items: center;
                }

                h1 {
                    margin-inline: auto;
                    color: black;
                    padding: .66rem;
                }

                p {
                    color: black;
                    display: flex;
                    justify-content: center;
                    align-items: center;
                    vertical-align: middle;
                    text-align: center;
                    white-space: pre-wrap;
                }

                footer {
                    display: flex;
                    flex-direction: column;
                    justify-content: center;
                    align-items: center;
                }
            </style>

            <body class="body">
                <div class="container">
                    <div class="surface">
                        <header>
                            <h1>{{heading}}</h1>
                        </header>
                        <main>
                            {{htmlBody}}
                        </main>
                    </div>
                    <div class="surface">
                        <footer>
                            <strong>{{applicationName}} {{DateTime.UtcNow.Year}}</strong>
                        </footer>
                    </div>
                </div>
            </body>
        </html>
    """;

    public static string MatchingCreatedMessage(string applicationName, string link) => Template
    (
        applicationName,
        "you've got a new match!",
        $$"""
            <p>
                Click <a href='{{link}}'>here</a> to see who you matched with!
            </p>
        """
    );

    public static string MessageCreatedMessage(string applicationName, string link) => Template
    (
        applicationName,
        "you've got a new message!",
        $$"""
            <p>
                Click <a href='{{link}}'>here</a> to see the message!
            </p>
        """
    );

    internal static string PositivelyJudgedMessage(string applicationName, string link) => Template
    (
        applicationName,
        "you've got a new like!",
        $$"""
            <p>
                Click <a href='{{link}}'>here</a> to see who liked you!
            </p>
        """
    );

    internal static string VerificationMessage(string applicationName, string code) => Template
    (
        applicationName,
        "verify your e-mail address",
        $$"""
            <p>
                Your e-mail verification code is <strong>{{code}}</strong>.
            </p>
        """
    );
}