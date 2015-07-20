/*
INSERT INTO [SmtpBox] ([Name],[Server],[Port],[SecureConnection],[Authenticate],[User],[Password],[IsDefault])
  VALUES (
    N'Default',   -- SMTP Box Name.
    N'localhost', -- SMTP Server Address.
    25,           -- SMTP Server Port.
    0,            -- Secure Connection Type. 0: None, 1: SSL 3.0, 2: TLS
    1,            -- Authenticate. 0: False, 1: True.
    N'user',      -- User Name for Authentication.
    N'password',  -- User Password for Authentication.
    1
  )
*/
