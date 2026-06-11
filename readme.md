# Testign the Power Platform API
The code in [Program.cs](./src/Program.cs) demonstrates basic authroization functionality to access the Power Platform APIs. It follows these steps:

1. Request a device code from Microsoft
2. Provide that device code to the user, ask them to login
3. After they do, request the access (bearer) token from Microsoft
4. Get that token, make API calls to the Power Platform API


## How to

### Set up App Registration in Azure
Allow Public client flows which allows use of the "Device Code" authorization:
![allow public client flows](https://i.imgur.com/NS5S5iU.png)

Add api.powerplatform.com as a permission for this client:
![add PowerApps Service to permissions = "api.powerplatform.com"](https://i.imgur.com/ZCFEWbR.png)

Details in my example:
- Tenant ID: `1a70d84e-bde9-4a11-ba5d-1305bb72ccb4`
- Client ID: `537c9fd5-1887-4786-9fc2-05863149de86`

### Get Device Code
```
POST https://login.microsoftonline.com/common/oauth2/v2.0/devicecode
Content-Type: application/x-www-form-urlencoded

client_id=537c9fd5-1887-4786-9fc2-05863149de86
&scope=https://api.powerplatform.com/.default offline_access
```

![get device code](https://i.imgur.com/VxCpATN.png)

### Poll Continuously
```
POST https://login.microsoftonline.com/common/oauth2/v2.0/token
Content-Type: application/x-www-form-urlencoded

grant_type=urn:ietf:params:oauth:grant-type:device_code
&client_id=537c9fd5-1887-4786-9fc2-05863149de86
&device_code=ABgABIQEAAAAdDD7nC9b5Q7JPd_okEQRFRXZvU3RzQXJ0aWZhY3RzAQAAAAAA6Mbp0ai6LX1WY6lWR7N97qwLX24GdiRFAmePdjkvw_QCaIyO69JNyebslytNwFuJoVfpIOn_5VYhRA_WOVgzN4fkj-XeOGXAo_5bFLbkUezpiR-4KvbAeWxfUL3sdkZVz9kb67B6-pZFfW-boZnQQr4RAJXgyCM1dtImxSx-9AsgTNvKVqAeUAao_9MHjmM47pU4yjYCoq2QaONs8aFSLAWvbG_DjW7fg3HxWlBLVyjgzQtDZ83dXwGz4Ji19sPLwsANrVlthfIjKx9GlXU84BB-8lIJBY9UeWivya1d47Lut068PDNiYa_fdMQquiJzW3VYO7jtqlWPD5_rddq68vEKPaNZXsZj3o16BwvZT-NTo_AHZU28cdojjxwq3YwRtiF7sVCLuMbS1JGfZrbwxW1PZmFru5BNO7Mi-wYq-X8N8wbHphQ0QAuoMbE9wLA76N_tlItYh_zuY70cXty1eNJMfHPG-x106Q10H2D0A488xJu6W0Ju2wpeJtdqIiut2kIaKNdDXKgdIWFQtVmB0YhoasImM-HL7fjctDDJNLp0_QvzuqK3awxfbfyeDfPqmEEij0YuPRJhkRVw4ZhImPdyOd2Sig9YOIRJcohPUmHjDwEcghw1uSJP4a8z-1ag8cPUfwpBfXBg_FYS9GBYVqXbD6VvRaIGchIEopW4_gztQe-DEe6yW3Bw2v2e48jXit8sLodvLLLB-4VZRxrc1grM33sH-kIdv_mgTwxylBogAA
```

While pending, it will return `400 Bad Request`:

![400 bad request](https://i.imgur.com/VhBg30P.png)


### Give them user code and verification URI
- User Code: `AEPAQGY6F`
- verification_uri: `https://login.microsoft.com/device`

### Poll Continuously... and get the token after they authenticate!
```
POST https://login.microsoftonline.com/common/oauth2/v2.0/token
Content-Type: application/x-www-form-urlencoded

grant_type=urn:ietf:params:oauth:grant-type:device_code
&client_id=537c9fd5-1887-4786-9fc2-05863149de86
&device_code=ABgABIQEAAAAdDD7nC9b5Q7JPd_okEQRFRXZvU3RzQXJ0aWZhY3RzAQAAAAAA6Mbp0ai6LX1WY6lWR7N97qwLX24GdiRFAmePdjkvw_QCaIyO69JNyebslytNwFuJoVfpIOn_5VYhRA_WOVgzN4fkj-XeOGXAo_5bFLbkUezpiR-4KvbAeWxfUL3sdkZVz9kb67B6-pZFfW-boZnQQr4RAJXgyCM1dtImxSx-9AsgTNvKVqAeUAao_9MHjmM47pU4yjYCoq2QaONs8aFSLAWvbG_DjW7fg3HxWlBLVyjgzQtDZ83dXwGz4Ji19sPLwsANrVlthfIjKx9GlXU84BB-8lIJBY9UeWivya1d47Lut068PDNiYa_fdMQquiJzW3VYO7jtqlWPD5_rddq68vEKPaNZXsZj3o16BwvZT-NTo_AHZU28cdojjxwq3YwRtiF7sVCLuMbS1JGfZrbwxW1PZmFru5BNO7Mi-wYq-X8N8wbHphQ0QAuoMbE9wLA76N_tlItYh_zuY70cXty1eNJMfHPG-x106Q10H2D0A488xJu6W0Ju2wpeJtdqIiut2kIaKNdDXKgdIWFQtVmB0YhoasImM-HL7fjctDDJNLp0_QvzuqK3awxfbfyeDfPqmEEij0YuPRJhkRVw4ZhImPdyOd2Sig9YOIRJcohPUmHjDwEcghw1uSJP4a8z-1ag8cPUfwpBfXBg_FYS9GBYVqXbD6VvRaIGchIEopW4_gztQe-DEe6yW3Bw2v2e48jXit8sLodvLLLB-4VZRxrc1grM33sH-kIdv_mgTwxylBogAA
```

And it will respond with:
```
{
  "token_type": "Bearer",
  "scope": "https://api.powerplatform.com/EnvironmentManagement.Environments.Read https://api.powerplatform.com/.default",
  "expires_in": 4487,
  "ext_expires_in": 4487,
  "access_token": "eyJ0eXAiOiJKV1QiLCJub25jZSI6InZZbVZjTW5pNkhtN3RkeE5GM19SUUcySS03RGJtd3FldS00azg2bmFLOTQiLCJhbGciOiJSUzI1NiIsIng1dCI6IndoMDZzRWt6TEhKNXNOTmFVeVJZMl82TzhLMCIsImtpZCI6IndoMDZzRWt6TEhKNXNOTmFVeVJZMl82TzhLMCJ9.eyJhdWQiOiJodHRwczovL2FwaS5wb3dlcnBsYXRmb3JtLmNvbSIsImlzcyI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0LzFhNzBkODRlLWJkZTktNGExMS1iYTVkLTEzMDViYjcyY2NiNC8iLCJpYXQiOjE3ODExMzgxMjIsIm5iZiI6MTc4MTEzODEyMiwiZXhwIjoxNzgxMTQyOTEwLCJhY2N0IjowLCJhY3IiOiIxIiwiYWlvIjoiQVlRQWUvOGNBQUFBMFhkNDRRNFY2TTNGMmdXQy9yMFRaZFBoNVgyOHV1TkYvM2lXY211d3lIa2EzTDlOOW1Kd1hUQmwzdVdwYjFuVWlId1NuK29HakkrOXhGU1RhVFlPbXFtNjRIOVloQndKWVIxdSs3aUMyV1FrL2VWeEFiZzlLSjZjODkvRmpDMXdyOUlXNktjeDdaQnFXRzZtcU84eFhUbXBCY0R0MVdIZXgzT2hyS2tIek1RPSIsImFtciI6WyJwd2QiLCJtZmEiXSwiYXBwaWQiOiI1MzdjOWZkNS0xODg3LTQ3ODYtOWZjMi0wNTg2MzE0OWRlODYiLCJhcHBpZGFjciI6IjAiLCJmYW1pbHlfbmFtZSI6IkFkbWluaXN0cmF0b3IiLCJnaXZlbl9uYW1lIjoiTU9EIiwiaWR0eXAiOiJ1c2VyIiwiaXBhZGRyIjoiMTcyLjIwMC43MC44OSIsIm5hbWUiOiJNT0QgQWRtaW5pc3RyYXRvciIsIm9pZCI6ImY3YTVlYjhmLTg1ZjYtNDg1MS04YzcxLTVmNDBjZGM5MTQ1YSIsInB1aWQiOiIxMDAzMjAwNTlDNjkwNDc0IiwicmgiOiIxLkFWSUFUdGh3R3VtOUVVcTZYUk1GdTNMTXRBVGdlSVhHcGVkR2tUNFM5WWtTMzBNQUFNbFNBQS4iLCJzY3AiOiJFbnZpcm9ubWVudE1hbmFnZW1lbnQuRW52aXJvbm1lbnRzLlJlYWQiLCJzaWQiOiIwMDVjY2RlYS0wYzhiLWJlMWYtMDYzZC02YTY3YzkzYmQ4NDMiLCJzaWduaW5fc3RhdGUiOlsia21zaSJdLCJzdWIiOiI2SnVaOGUydUJzWDlCazNQWlNuWEhLODItWGh0MGRoc3A1U0syOS1xbF9JIiwidGlkIjoiMWE3MGQ4NGUtYmRlOS00YTExLWJhNWQtMTMwNWJiNzJjY2I0IiwidW5pcXVlX25hbWUiOiJhZG1pbkBNMzY1eDg5MDE1MDM5Lm9ubWljcm9zb2Z0LmNvbSIsInVwbiI6ImFkbWluQE0zNjV4ODkwMTUwMzkub25taWNyb3NvZnQuY29tIiwidXRpIjoiQjB4OVRvRmN6VW1fN0ZSMHNzSVJBQSIsInZlciI6IjEuMCIsIndpZHMiOlsiNjJlOTAzOTQtNjlmNS00MjM3LTkxOTAtMDEyMTc3MTQ1ZTEwIiwiZTA3NDk0YWQtMTY1NC00ZGQyLTkyMmUtNmY4MWE3MWJmMDBmIiwiYjc5ZmJmNGQtM2VmOS00Njg5LTgxNDMtNzZiMTk0ZTg1NTA5Il0sInhtc19hY3RfZmN0IjoiMyA5IiwieG1zX2Z0ZCI6IldQUGFYZ2ZxdmNHalkwYTM1bXRVOG5DTnJWX2tLZUtwR2xYd2l6MzVBVThCZFhOM1pYTjBNeTFrYzIxeiIsInhtc19pZHJlbCI6IjEgMjQiLCJ4bXNfc3ViX2ZjdCI6IjggMyJ9.bSc7oqqm_N-Ysu0z6YHSDNTjOiaR_bSPRrTsxZbqVTxbpHdrlU9qu1ErfbdwKPivItTOwtea033ZOOse00v93Q7fyBUop2Ng5_-FAGwzNKyouePbJqFfA625wYJH0AbfGD1Cfdug_CArxO0LUzZLmEsu-NshWYSE8EqicbqSAXckbykIYB9lFvPozJAQiQ6EnZmoR1bwN6odF8QZ5QPcHpX0N-skASNDQcvZsjwdFeUmTXbv6s734eHg_4BJbzNjSfnuPZhUq88BlakowyyJzDgqLPp0r7ZRx6h0h--sXOMfwJPIquN1LWmu7sXwyi5o2gSAtXgbb2YQA_cwO7YIqA",
  "refresh_token": "1.AVIATthwGum9EUq6XRMFu3LMtNWffFOHGIZHn8IFhjFJ3oYAAMlSAA.BQABAwEAAAADAOz_BQD0_0V2b1N0c0FydGlmYWN0cwIAAAAAAD4GNEfTHE86Q5Cn1D4elCEQtPMAKXw2Epquy1wgaxdhspyOSONG0PgUUNiY-diLK0VF-ytOVoH0WsVC084oUoWxGZO0pX5uGF4JB-vkCVsCRSvS5cCXej50o-4T97-goeVaK8y1MIdGP_YS3zW_ADlZD0lZKbwPDTur20JQGW_KImODwcsKVEomqV7j05inp6B4lFSmEJWuFFpQp86vKv8kcOZgIDzge4Q5m-pjQr5bLsmUkqe9W1vC8Sa-8UA5k0bmQZiI3Eg9BA5ngkiXbYHSkK-GfsjSk62qs-CGOPMs-jJwu6hQgvcBcaqrW689Zi6Lys5nzJUsZc3toIOg4AHp0HQqI1KVXcCGLETGw6SFRpPecsb-wYSn0NePDxB36eXryKvlaRhn9cJOvEJ7hVCiDQFocyp0hNmR1SRbbEuCbfzzu75ehz29CIvzDPpB5dTO89-StnLbOsDi8T5B40YwCCObP4H_RORfRn4vIR4YGMsi4Wm9fg3J8Uct-S6TIoIOyj_cNx-4Wq-RY37eIEEl85YZBuUAmLe1VwCrR3siN6O9ewQiCrCscfIYakDAzhOBSCnAN13QwPnEbHxzBYv8AeN6zM2yU8mT6BlP7sj5YENZrnAGLNikoR5-JNB8JWet6MRxMUw_3tYzPyAXfJ85VpW-BzY2GipbYuBs0AFUeDm6T7GKhSnworwb9hT1tWFiMGi8hwD5VllP8j98INsGTgJyedCOpCWj4cPHFp5ob-eJljfZsLHHoHIGL2JCjCkDQ_xvbBrdiGLbgJZ3zAQcVmjr8Uc_rBbbDd5yEpZuoCL-Dd9vSjiWtAaTOQQsluUeFlWMOaKSzsTOZrDMufYTCA54Qt8r7MBhpK6gqFy0nO1aREisL-JBEkONZUOlBUaQANadGUuzFjrXJxFr5fiTYNbfVuBqlpA-iyasUz3ygk5RIv2HmuvCltLNk6la7IX4eQQLkPzcW9rTNparbQsoXhH-EdnAzUwReLkjvttvSgGxRBji1wgWMnl0WcY2QfviWBM92UTR3Ulq49rSYJV53tKvt_LyPaqwVTHzZjwTeAEEDKfHZYBqgA0RVvNuKhu6bQK0jtbO1xcik4dJNcmMFWsAzWx7F_KT7r9lVxFvB-Jk6a7JMVUdldynsn-plOCJ75JrGOCcQSibl0jOUHzm10dFpG2mAuMre-kro0epc8IwevQ8mMWmcVArP39gdeSfa7FMGxYao2qt2xVncj7tehA9BW1fPidzYpAkMPuuVxX1w18bgLDwFaVk3AiC4TIt3soqFb6KWdnfN7VubNVD9nUTKgvBQFdlS4L0FOvJzD-qNMLhbhzBOLpwevM_40MCes0t-a9mBWb8DR9Oxv7u1r9P6mSkzcQDBMk77pc"
}
```

![token](https://i.imgur.com/7A5oR7S.png)

### List Environments
```
GET https://api.powerplatform.com/environmentmanagement/environments?api-version=2024-10-01
Authorization: Bearer eyJ0eXAiOiJKV1Qi...
```

Will respond with:

```
{
  "value": [
    {
      "id": "Default-1a70d84e-bde9-4a11-ba5d-1305bb72ccb4",
      "displayName": "Contoso (default)",
      "tenantId": "1a70d84e-bde9-4a11-ba5d-1305bb72ccb4",
      "type": "Default",
      "geo": "unitedstates",
      "azureRegion": "westus",
      "createdDateTime": "2026-02-19T20:29:07.5632642Z",
      "deletedDateTime": "1970-01-01T00:00:00Z",
      "dataverseId": "dbf2efc3-b452-f111-b31f-6045bd0a4b1b",
      "url": "https://org1efb1085.crm.dynamics.com",
      "version": "9.2.26051.157",
      "domainName": "org1efb1085",
      "state": "Enabled",
      "adminMode": "Disabled",
      "backgroundOperationsState": "Enabled",
      "protectionLevel": "Basic",
      "createdBy": {
        "id": "SYSTEM",
        "type": "NotSpecified"
      },
      "retentionDetails": {
        "retentionPeriod": "P7D",
        "availableFromDateTime": "2026-06-04T00:44:57.8295216Z"
      }
    },
    {
      "id": "3392c536-93cd-e2b0-a25d-5d84503ad914",
      "displayName": "timh testing",
      "tenantId": "1a70d84e-bde9-4a11-ba5d-1305bb72ccb4",
      "type": "SubscriptionBasedTrial",
      "geo": "unitedstates",
      "azureRegion": "westus",
      "createdDateTime": "2026-03-11T22:11:18.2327907Z",
      "deletedDateTime": "1970-01-01T00:00:00Z",
      "dataverseId": "77ba86d6-951d-f111-afc0-6045bd003e37",
      "url": "https://org53194471.crm.dynamics.com",
      "version": "9.2.26051.157",
      "domainName": "org53194471",
      "state": "Enabled",
      "adminMode": "Disabled",
      "backgroundOperationsState": "Enabled",
      "protectionLevel": "Basic",
      "createdBy": {
        "id": "f7a5eb8f-85f6-4851-8c71-5f40cdc9145a",
        "type": "User"
      },
      "retentionDetails": {
        "retentionPeriod": "P7D",
        "availableFromDateTime": "2026-06-04T00:44:57.8295244Z"
      }
    },
    {
      "id": "b8f7286d-4ffb-ebe7-9125-4aebfd79b603",
      "displayName": "jb",
      "tenantId": "1a70d84e-bde9-4a11-ba5d-1305bb72ccb4",
      "type": "Trial",
      "geo": "unitedstates",
      "azureRegion": "westus",
      "createdDateTime": "2026-05-18T18:11:06.2289348Z",
      "deletedDateTime": "1970-01-01T00:00:00Z",
      "dataverseId": "428ef692-e352-f111-b31f-6045bd003332",
      "url": "https://org3f223c59.crm.dynamics.com",
      "version": "9.2.26051.157",
      "domainName": "org3f223c59",
      "state": "Enabled",
      "adminMode": "Disabled",
      "backgroundOperationsState": "Enabled",
      "protectionLevel": "Basic",
      "createdBy": {
        "id": "f7a5eb8f-85f6-4851-8c71-5f40cdc9145a",
        "type": "User"
      },
      "retentionDetails": {
        "retentionPeriod": "P7D",
        "availableFromDateTime": "2026-06-04T00:44:57.8295283Z"
      }
    },
    {
      "id": "691a16de-3b74-e7bb-93c3-52f8420de007",
      "displayName": "PowerPagesDeveloper-051826-141628",
      "tenantId": "1a70d84e-bde9-4a11-ba5d-1305bb72ccb4",
      "type": "Developer",
      "geo": "unitedstates",
      "azureRegion": "westus",
      "createdDateTime": "2026-05-18T18:16:30.3171428Z",
      "deletedDateTime": "1970-01-01T00:00:00Z",
      "dataverseId": "3de6bbf8-e452-f111-b31f-6045bd0a4b26",
      "url": "https://org9faee8d2.crm.dynamics.com",
      "version": "9.2.26051.157",
      "domainName": "org9faee8d2",
      "state": "Enabled",
      "adminMode": "Disabled",
      "backgroundOperationsState": "Enabled",
      "protectionLevel": "Basic",
      "createdBy": {
        "id": "f7a5eb8f-85f6-4851-8c71-5f40cdc9145a",
        "type": "User"
      },
      "createdFor": {
        "id": "f7a5eb8f-85f6-4851-8c71-5f40cdc9145a",
        "type": "User"
      },
      "retentionDetails": {
        "retentionPeriod": "P7D",
        "availableFromDateTime": "2026-06-04T00:44:57.8295298Z"
      }
    },
    {
      "id": "c1347f22-7b53-e693-bcc5-bca264d75502",
      "displayName": "PowerPagesDeveloper-051826-142021",
      "tenantId": "1a70d84e-bde9-4a11-ba5d-1305bb72ccb4",
      "type": "Developer",
      "geo": "unitedstates",
      "azureRegion": "westus",
      "createdDateTime": "2026-05-18T18:20:22.9093504Z",
      "deletedDateTime": "1970-01-01T00:00:00Z",
      "dataverseId": "28e6bbf8-e452-f111-b31f-6045bd0a4b2e",
      "url": "https://orgad7d9dfa.crm.dynamics.com",
      "version": "9.2.26051.157",
      "domainName": "orgad7d9dfa",
      "state": "Enabled",
      "adminMode": "Disabled",
      "backgroundOperationsState": "Enabled",
      "protectionLevel": "Basic",
      "createdBy": {
        "id": "f7a5eb8f-85f6-4851-8c71-5f40cdc9145a",
        "type": "User"
      },
      "createdFor": {
        "id": "f7a5eb8f-85f6-4851-8c71-5f40cdc9145a",
        "type": "User"
      },
      "retentionDetails": {
        "retentionPeriod": "P7D",
        "availableFromDateTime": "2026-06-04T00:44:57.8295312Z"
      }
    },
    {
      "id": "d79dae6e-3ba1-ebc7-9275-4d5d3f36710c",
      "displayName": "PowerPagesDeveloper-051826-142248",
      "tenantId": "1a70d84e-bde9-4a11-ba5d-1305bb72ccb4",
      "type": "Developer",
      "geo": "unitedstates",
      "azureRegion": "westus",
      "createdDateTime": "2026-05-18T18:22:50.4822377Z",
      "deletedDateTime": "1970-01-01T00:00:00Z",
      "dataverseId": "3f36e096-e652-f111-b31d-6045bd05eb03",
      "url": "https://powerpagesdeveloper-051826-14224.crm.dynamics.com",
      "version": "9.2.26051.157",
      "domainName": "powerpagesdeveloper-051826-14224",
      "state": "Enabled",
      "adminMode": "Disabled",
      "backgroundOperationsState": "Enabled",
      "protectionLevel": "Basic",
      "createdBy": {
        "id": "f7a5eb8f-85f6-4851-8c71-5f40cdc9145a",
        "type": "User"
      },
      "createdFor": {
        "id": "f7a5eb8f-85f6-4851-8c71-5f40cdc9145a",
        "type": "User"
      },
      "retentionDetails": {
        "retentionPeriod": "P7D",
        "availableFromDateTime": "2026-06-04T00:44:57.8295336Z"
      }
    }
  ]
}
```

![list enviornments response](https://i.imgur.com/zpnpI9i.png)

