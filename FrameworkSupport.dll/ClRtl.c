Source file: Microsoft.FailoverClusters.FrameworkSupport
Generated time: 20260130_193657
==========

FUNCTION: ClRtlGetClusterConnectionInformation @ 0x18001D850
----------
__int64 __fastcall ClRtlGetClusterConnectionInformation(__int64 a1, int a2, wchar_t *a3, unsigned int *a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v7 = 0;
  if ( !a1 || !a4 )
    return 87;
  if ( a2 )
  {
    if ( a2 != 1 )
      return 87;
    v8 = (struct _RTL_CRITICAL_SECTION *)(a1 + 232);
    EnterCriticalSection((LPCRITICAL_SECTION)(a1 + 232));
    v9 = *(const wchar_t **)(a1 + 24);
    v10 = -1i64;
    do
      ++v10;
    while ( v9[v10] );
    v11 = v10 + 1;
    v12 = 2 * v11;
    if ( a3 && *a4 >= v12 )
    {
      v13 = StringCchCopyW(a3, v11, v9);
      if ( v13 /*signed*/< 0 )
      {
        if ( (v13 & 0x1FFF0000) == 458752 )
          v7 = (unsigned __int16)v13;
        else
          v7 = v13;
      }
    }
    else
    {
      v7 = 122;
    }
    LeaveCriticalSection(v8);
    goto LABEL_19;
  }
  v12 = 4;
  if ( a3 && *a4 >= 4 )
  {
    v14 = (struct _RTL_CRITICAL_SECTION *)(a1 + 232);
    EnterCriticalSection((LPCRITICAL_SECTION)(a1 + 232));
    *(_DWORD *)a3 = *(_DWORD *)(a1 + 320);
    LeaveCriticalSection(v14);
LABEL_21:
    *a4 = v12;
    return v7;
  }
  v7 = 122;
LABEL_19:
  if ( !v7 || v7 == 122 )
    goto LABEL_21;
  return v7;
}


==========

FUNCTION: ?ClRtlFindDCAndSearchForObject@@YAKPEB_W00KPEAPEAU_DOMAIN_CONTROLLER_INFOW@@PEAPEA_WPEAH@Z @ 0x18001D984
----------
unsigned int __fastcall ClRtlFindDCAndSearchForObject(const wchar_t *a1, const wchar_t *a2, const wchar_t *a3, unsigned int a4, struct _DOMAIN_CONTROLLER_INFOW **a5, wchar_t **phDS, int *a7)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v14 = 0i64;
  v15 = 0;
  v13 = 0i64;
  hMem = 0i64;
  v8 = DsGetDcNameW(0i64, a3, 0i64, 0i64, a4, &v14);
  if ( v8 )
    return v8;
  if ( !v14 )
    return 8240;
  phDS = 0i64;
  v8 = DsBindW(v14->DomainControllerName, 0i64, (HANDLE *)&phDS);
  if ( !v8 )
  {
    DsUnBindW((HANDLE *)&phDS);
    v10 = ClRtlIsComputerObjectInDS(v9, (__int64)a2, v14->DomainControllerName, &v15, &v13, &hMem);
    if ( v10 /*signed*/< 0 )
    {
      v8 = (unsigned __int16)v10;
      if ( (v10 & 0x1FFF0000) != 458752 )
        v8 = v10;
    }
    else
    {
      *a7 = v15;
      *a5 = v14;
    }
    if ( hMem )
      LocalFree(hMem);
  }
  return v8;
}


==========

FUNCTION: ClRtlAreClaimsEnabledInDomain @ 0x18001DC14
----------
__int64 __fastcall ClRtlAreClaimsEnabledInDomain(wchar_t *a1, _DWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  LsaHandle = 0i64;
  AuthenticationPackage = 0;
  ProtocolStatus = 0;
  LODWORD(v4) = 0;
  Buffer = 0i64;
  v5 = 24;
  *a2 = 0;
  DestinationString = 0i64;
  if ( a1 )
  {
    v6 = -1i64;
    v7 = -1i64;
    do
      ++v7;
    while ( a1[v7] );
    v5 = 2 * v7 + 26;
    v8 = (wchar_t *)LocalAlloc(0x40u, v5);
    v9 = v8;
    if ( !v8 )
      goto LABEL_5;
    *(_QWORD *)v8 = 32i64;
    do
      ++v6;
    while ( a1[v6] );
    StringCchCopyW(v8 + 12, v6 + 1, a1);
  }
  else
  {
    v12 = (wchar_t *)LocalAlloc(0x40u, 0x18ui64);
    v9 = v12;
    if ( !v12 )
    {
LABEL_5:
      v10 = GetLastError();
      ClRtlLogPrint(1u, "ClRtlAreClaimsEnabledInDomain - Memory alloc of size %1!d! failed, status %2!d!.\n", v5, v10);
      return (unsigned int)v10;
    }
    *(_QWORD *)v12 = 32i64;
  }
  RtlInitUnicodeString((PUNICODE_STRING)(v9 + 4), 0i64);
  v13 = LsaConnectUntrusted(&LsaHandle);
  if ( v13 /*signed*/< 0 )
  {
    v4 = RtlNtStatusToDosError(v13);
    ClRtlLogPrint(1u, "ClRtlAreClaimsEnabledInDomain - Untrusted connection to LSA failed, status %1!d!.\n", v4);
    goto LABEL_25;
  }
  RtlInitString(&DestinationString, "Kerberos");
  v14 = LsaLookupAuthenticationPackage(LsaHandle, (PLSA_STRING)&DestinationString, &AuthenticationPackage);
  if ( v14 /*signed*/< 0 )
  {
    v4 = RtlNtStatusToDosError(v14);
    ClRtlLogPrint(1u, "ClRtlAreClaimsEnabledInDomain - Looking up kerb auth package failed, status %1!d!.\n", v4);
  }
  else
  {
    ReturnBufferLength = 0;
    v15 = LsaCallAuthenticationPackage(LsaHandle, AuthenticationPackage, v9, v5, &Buffer, &ReturnBufferLength, &ProtocolStatus);
    if ( v15 /*signed*/< 0 || (v15 = ProtocolStatus, ProtocolStatus /*signed*/< 0) )
    {
      v4 = RtlNtStatusToDosError(v15);
      ClRtlLogPrint(1u, "ClRtlAreClaimsEnabledInDomain - Calling kerb auth package failed, status %1!d!.\n", v4);
      v16 = Buffer;
LABEL_20:
      if ( v16 )
      {
        LsaFreeReturnBuffer(v16);
        Buffer = 0i64;
      }
      goto LABEL_23;
    }
    v16 = Buffer;
    if ( Buffer )
    {
      v17 = 0;
      if ( (*((_BYTE *)Buffer + 4) & 1) == 0 )
        v17 = *((_DWORD *)Buffer + 3);
      *a2 = v17;
      goto LABEL_20;
    }
  }
LABEL_23:
  LsaDeregisterLogonProcess(LsaHandle);
LABEL_25:
  LocalFree(v9);
  return (unsigned int)v4;
}


==========

FUNCTION: ClRtlFindDomainForServer @ 0x18001DE84
----------
__int64 __fastcall ClRtlFindDomainForServer(void *a1, __int64 a2, __int64 a3, PDOMAIN_CONTROLLER_INFOW *a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  phDS = a1;
  DomainControllerInfo = 0i64;
  for ( Flags = 1073741840; ; Flags |= 1u )
  {
    v6 = DsGetDcNameW(0i64, 0i64, 0i64, 0i64, Flags, &DomainControllerInfo);
    if ( v6 )
      break;
    if ( !DomainControllerInfo )
      return 8240;
    phDS = 0i64;
    v6 = DsBindW(DomainControllerInfo->DomainControllerName, 0i64, &phDS);
    if ( !v6 )
    {
      DsUnBindW(&phDS);
      *a4 = DomainControllerInfo;
      return v6;
    }
    NetApiBufferFree(DomainControllerInfo);
    if ( (Flags & 1) != 0 )
      return v6;
    DomainControllerInfo = 0i64;
  }
  return v6;
}


==========

FUNCTION: ClRtlFindSuitableDCInfo @ 0x18001DF5C
----------
__int64 __fastcall ClRtlFindSuitableDCInfo(const wchar_t *a1, wchar_t *a2, wchar_t *a3, __int64 a4, int a5, struct _DOMAIN_CONTROLLER_INFOW **a6, int a7, int *a8, _DWORD *a9)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v29 = a2;
  v28 = HIDWORD(a1);
  v9 = 0i64;
  v30 = 0;
  v12 = 0;
  v27 = 0;
  v13 = 1073745921;
  *a9 = 0;
  Buffer = 0i64;
  v25 = 0i64;
  a7 = 0;
  if ( !a5 )
  {
    if ( !(unsigned int)ClRtlAreClaimsEnabledInDomain(a3, &a7) && a7 )
      v13 = a7 | 0x40001001;
    else
      v13 = 1073745937;
  }
  v14 = ClRtlFindDCAndSearchForObject(a1, a2, a3, v13, (struct _DOMAIN_CONTROLLER_INFOW **)&Buffer, v23, &v30);
  v16 = a6;
  v17 = (struct _DOMAIN_CONTROLLER_INFOW *)Buffer;
  if ( !v14 && v30 )
    goto LABEL_11;
  v18 = ClRtlFindDCAndSearchForObject(v15, v29, a3, v13 & 0xFFFFEFFE, &v25, v24, &v27);
  if ( !v18 )
  {
    v12 = v27;
    if ( !v30 )
    {
      if ( v27 || !v17 )
      {
        v9 = v25;
        v19 = v25;
        *v16 = v25;
      }
      else
      {
        v9 = v25;
        v19 = v17;
        *v16 = v17;
      }
LABEL_16:
      v20 = a9;
      v21 = v19->DomainControllerName;
      *a8 = v30 | v12;
      v18 = ClRtlIsDomainControllerReadOnly(v21, v20);
      if ( !v18 )
        goto LABEL_24;
      goto LABEL_19;
    }
    v9 = v25;
LABEL_11:
    *v16 = v17;
    v19 = v17;
    goto LABEL_16;
  }
  v9 = v25;
LABEL_19:
  if ( v9 )
  {
    NetApiBufferFree(v9);
    v9 = 0i64;
  }
  if ( v17 )
  {
    NetApiBufferFree(v17);
    v17 = 0i64;
  }
  *v16 = 0i64;
LABEL_24:
  if ( v9 && *v16 != v9 )
    NetApiBufferFree(v9);
  if ( v17 && *v16 != v17 )
    NetApiBufferFree(v17);
  return v18;
}


==========

FUNCTION: ClRtlGenerateRandomBytes @ 0x18001E108
----------
DWORD __fastcall ClRtlGenerateRandomBytes(BYTE *pbBuffer, int a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = 0;
  hProv = 0i64;
  v4 = a2 - 1;
  if ( !CryptAcquireContextW(&hProv, 0i64, 0i64, 1u, 0xF0000000) )
    return GetLastError();
  if ( CryptGenRandom(hProv, 2 * v4, pbBuffer) )
  {
    while ( v4 )
    {
      v6 = *(_WORD *)pbBuffer;
      --v4;
      if ( (unsigned __int16)(*(_WORD *)pbBuffer + 10240) > 0x7FFu )
      {
        if ( !v6 )
          *(_WORD *)pbBuffer = -23563;
      }
      else
      {
        *(_WORD *)pbBuffer = v6 & 0x7FFF;
      }
      pbBuffer += 2;
    }
    *(_WORD *)pbBuffer = 0;
  }
  else
  {
    v2 = GetLastError();
  }
  CryptReleaseContext(hProv, 0);
  return v2;
}


==========

FUNCTION: ClRtlGetADDomainEx @ 0x18001E1E8
----------
__int64 __fastcall ClRtlGetADDomainEx(__int64 a1, __int64 *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  Buffer = 0i64;
  v3 = DsRoleGetPrimaryDomainInformation(0i64, DsRolePrimaryDomainInfoBasic, (PBYTE *)&Buffer);
  if ( v3 )
    return v3;
  v4 = Buffer;
  if ( (*(_DWORD *)Buffer & 0xFFFFFFFD) != 0 )
  {
    if ( a2 )
    {
      v5 = ClRtlDupString(*((wchar_t **)Buffer + 2));
      v4 = Buffer;
      *a2 = (__int64)v5;
    }
  }
  else if ( a2 )
  {
    *a2 = 0i64;
  }
  DsRoleFreeMemory(v4);
  return v3;
}


==========

FUNCTION: ClRtlGetResourceReadOnlyPrivateProps @ 0x18001E26C
----------
__int64 __fastcall ClRtlGetResourceReadOnlyPrivateProps(HRESOURCE hResource, _QWORD *a2, _DWORD *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  lpOutBuffer = 0i64;
  LODWORD(uBytes) = 0;
  v7 = ClusterResourceControl(hResource, 0i64, 0x100007Du, 0i64, 0, 0i64, 0, (LPDWORD)&uBytes);
  if ( v7 || ((lpOutBuffer = LocalAlloc(0, (unsigned int)uBytes)) == 0i64 ? (v8 = GetLastError()) : (v8 = ClusterResourceControl(hResource, 0i64, 0x100007Du, 0i64, 0, lpOutBuffer, uBytes, (LPDWORD)&uBytes)), (v7 = v8) != 0) )
  {
    LocalFree(lpOutBuffer);
  }
  else
  {
    *a3 = uBytes;
    *a2 = lpOutBuffer;
  }
  return v7;
}


==========

FUNCTION: ClRtlIsComputerObjectInDS @ 0x18001E358
----------
__int64 __fastcall ClRtlIsComputerObjectInDS(void *a1, __int64 a2, WCHAR *a3, _DWORD *a4, wchar_t **a5, _QWORD *a6)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v6 = szPathName;
  v7 = a3;
  v35 = *(_OWORD *)L"LDAP://";
  v8 = 1;
  v28 = (wchar_t **)a6;
  wcscpy((wchar_t *)v36, L"(&(objectCategory=computer)(samAccountName=");
  v9 = 0i64;
  v25 = L"distinguishedName";
  wcscpy((wchar_t *)v34, L"))");
  v27 = a4;
  v26 = a2;
  v32 = 0i64;
  ppObject = 0i64;
  v24 = 0i64;
  v23 = 0i64;
  v30 = 0i64;
  v31 = 0i64;
  if ( a3 )
  {
    if ( *a3 == 92 && a3[1] == 92 )
      v7 = a3 + 2;
    v8 = 513;
    goto LABEL_10;
  }
  result = ClRtlFindDomainForServer(a1, a2, 0i64, &v23);
  if ( !(_DWORD)result )
  {
    v9 = v23;
    v7 = v23->DomainName;
LABEL_10:
    v11 = -1i64;
    v12 = -1i64;
    do
      ++v12;
    while ( v7[v12] );
    v13 = (unsigned int)(v12 + 8);
    v14 = (unsigned int)v13;
    if ( (unsigned int)v13 <= 0x108 || (v6 = (wchar_t *)LocalAlloc(0, 2 * v13)) != 0i64 )
    {
      StringCchPrintfW(v6, v14, L"%ws%ws", &v35, v7);
      v16 = ADsOpenObject(v6, 0i64, 0i64, v8, &IID_IDirectorySearch, &ppObject);
      if ( v16 /*signed*/>= 0 )
      {
        v33[0] = 2;
        v33[2] = 7;
        v33[12] = 7;
        v33[4] = 1;
        v33[10] = 5;
        v33[14] = 2;
        v16 = (*(__int64 (__fastcall **)(void *, int *))(*(_QWORD *)ppObject + 24i64))(ppObject, v33);
        if ( v16 /*signed*/>= 0 )
        {
          LODWORD(riid) = 15;
          StringCchPrintfW(v37, 0x3Fui64, L"%ws%.*ws$%ws", v36, riid, v26, v34);
          v16 = (*(__int64 (__fastcall **)(void *, wchar_t *, const wchar_t **, __int64, __int64 *))(*(_QWORD *)ppObject + 32i64))(ppObject, v37, &v25, 1i64, &v24);
          if ( v16 /*signed*/>= 0 )
          {
            v16 = (*(__int64 (__fastcall **)(void *, __int64))(*(_QWORD *)ppObject + 48i64))(ppObject, v24);
            *v27 = v16 == 0;
            if ( v16 == 20498 )
            {
              v16 = 0;
            }
            else if ( !v16 )
            {
              if ( a5 )
              {
                v16 = (*(__int64 (__fastcall **)(void *, __int64, const wchar_t *, __int128 *))(*(_QWORD *)ppObject + 80i64))(ppObject, v24, v25, &v30);
                if ( v16 /*signed*/>= 0 )
                {
                  do
                    ++v11;
                  while ( *(_WORD *)(*(_QWORD *)(v31 + 8) + 2 * v11) );
                  v17 = (unsigned int)(v11 + 1);
                  v18 = (wchar_t *)LocalAlloc(0, 2 * v17);
                  *a5 = v18;
                  if ( v18 )
                  {
                    StringCchCopyW(v18, (unsigned int)v17, *(const wchar_t **)(v31 + 8));
                  }
                  else
                  {
                    v19 = GetLastError();
                    v16 = v19;
                    if ( v19 /*signed*/> 0 )
                      v16 = (unsigned __int16)v19 | 0x80070000;
                  }
                  (*(void (__fastcall **)(void *, __int128 *))(*(_QWORD *)ppObject + 88i64))(ppObject, &v30);
                }
              }
              v20 = v28;
              if ( v28 )
              {
                v23 = 0i64;
                *(_OWORD *)&pvarg.vt = 0i64;
                pvarg.pRecInfo = 0i64;
                VariantInit(&pvarg);
                v16 = (**(__int64 (__fastcall ***)(void *, GUID *, PDOMAIN_CONTROLLER_INFOW *))ppObject)(ppObject, &IID_IADsObjectOptions, &v23);
                if ( v16 /*signed*/>= 0 )
                {
                  v16 = (*((__int64 (__fastcall **)(PDOMAIN_CONTROLLER_INFOW, _QWORD, VARIANTARG *))v23->DomainControllerName + 7))(v23, 0i64, &pvarg);
                  if ( v16 /*signed*/>= 0 )
                    *v20 = ClRtlDupString(pvarg.bstrVal);
                  VariantClear(&pvarg);
                }
                if ( v23 )
                  (*((void (__fastcall **)(PDOMAIN_CONTROLLER_INFOW))v23->DomainControllerName + 2))(v23);
              }
            }
            (*(void (__fastcall **)(void *, __int64))(*(_QWORD *)ppObject + 96i64))(ppObject, v24);
          }
        }
      }
    }
    else
    {
      v15 = GetLastError();
      v16 = v15;
      if ( v15 /*signed*/> 0 )
        v16 = (unsigned __int16)v15 | 0x80070000;
    }
    if ( ppObject )
      (*(void (__fastcall **)(void *))(*(_QWORD *)ppObject + 16i64))(ppObject);
    if ( v9 )
      NetApiBufferFree(v9);
    if ( v6 == szPathName )
      return (unsigned int)v16;
    if ( v6 )
      LocalFree(v6);
    return (unsigned int)v16;
  }
  if ( (int)result /*signed*/> 0 )
    result = (unsigned __int16)result | 0x80070000;
  return result;
}


==========

FUNCTION: ClRtlIsDomainControllerReadOnly @ 0x18001E810
----------
__int64 __fastcall ClRtlIsDomainControllerReadOnly(const WCHAR *a1, int *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a2 = 0;
  Buffer = 0i64;
  v3 = DsRoleGetPrimaryDomainInformation(a1, DsRolePrimaryDomainInfoBasic, &Buffer);
  v4 = Buffer;
  v5 = v3;
  if ( !v3 )
  {
    if ( !Buffer )
      return v5;
    *a2 = *((_DWORD *)Buffer + 1) & 8;
  }
  if ( v4 )
    DsRoleFreeMemory(v4);
  return v5;
}


==========

FUNCTION: ClRtlKerbSetPassword @ 0x18001E87C
----------
__int64 __fastcall ClRtlKerbSetPassword(PCWSTR a1, LPCWSTR lpServer, PCWSTR a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = 0i64;
  v4 = 0i64;
  if ( lpServer && *lpServer == 92 && lpServer[1] == 92 )
    v3 = lpServer + 2;
  Buffer = 0i64;
  v7 = DsRoleGetPrimaryDomainInformation(lpServer, DsRolePrimaryDomainInfoBasic, &Buffer);
  if ( !v7 )
  {
    if ( !Buffer )
      goto LABEL_10;
    v4 = ClRtlDupString(*((wchar_t **)Buffer + 2));
  }
  if ( Buffer )
    DsRoleFreeMemory(Buffer);
LABEL_10:
  if ( !v7 )
  {
    v9 = KerbSetPasswordUserEx(v4, a1, a3, v8, v3);
    if ( v9 /*signed*/< 0 )
      v7 = RtlNtStatusToDosError(v9);
  }
  if ( v4 )
    LocalFree(v4);
  return v7;
}


==========

FUNCTION: ClRtlResetPassword @ 0x18001E95C
----------
__int64 __fastcall ClRtlResetPassword(HRESOURCE hResource, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v36 = (wchar_t *)a2;
  hResourcea = hResource;
  phDS = 0i64;
  v3 = 0i64;
  hMem = 0i64;
  v4 = 0i64;
  Buffer = 0i64;
  v5 = 0i64;
  v33 = 0i64;
  v6 = 0i64;
  v30 = 0;
  v31 = 0;
  v35 = 0i64;
  v28 = 0;
  if ( !a2 || (unsigned __int64)hResource - 1 > 0xFFFFFFFFFFFFFFFDui64 )
    return 87;
  v7 = GetClusterFromResource(hResource);
  v39 = v7;
  v8 = v7;
  if ( v7 )
  {
    v29 = 0;
    HIDWORD(uBytes) = 0;
    v9 = GetClusterFunctionalLevel(v7, &v29, (unsigned int *)&uBytes + 1);
    if ( v9 )
      goto LABEL_5;
    if ( v29 > 0xB || v29 == 11 && HIDWORD(uBytes) >= 2 )
    {
      v31 = 1;
      uBytes = 0x8000000100i64;
    }
    else
    {
      uBytes = 0xF0000001Ei64;
    }
    v11 = v36;
    StringCchPrintfW(v42, 0x11ui64, L"%.*ws$", 15i64, v36);
    v12 = ClRtlGetResourceReadOnlyPrivateProps(hResource, &v35, &v28);
    v6 = v35;
    if ( !v12 )
    {
      v15 = ClRtlpFindSzProperty(v35, v28, v14, (wchar_t **)&Buffer);
      v3 = (WCHAR *)Buffer;
      if ( v15 )
        v3 = 0i64;
    }
LABEL_23:
    if ( v3 )
    {
      v16 = 1;
      goto LABEL_32;
    }
    while ( 1 )
    {
      v28 = 0;
      v16 = 0;
      v17 = ClRtlGetADDomainEx((__int64)v13, (__int64 *)&v33);
      v4 = v33;
      v9 = v17;
      if ( v17 )
        goto LABEL_41;
      Buffer = 0i64;
      v3 = 0i64;
      v19 = ClRtlFindSuitableDCInfo(0i64, v11, v33, v18, 0, (struct _DOMAIN_CONTROLLER_INFOW **)&Buffer, cbOutBufferSize, &v30, &v28);
      v20 = Buffer;
      v9 = v19;
      if ( !v19 )
        v3 = ClRtlDupString(*(wchar_t **)Buffer);
      if ( v20 )
        NetApiBufferFree(v20);
      if ( v9 )
        goto LABEL_41;
      v11 = v36;
LABEL_32:
      v9 = DsBindW(v3, 0i64, &phDS);
      if ( !v9 )
        break;
      if ( !v16 )
        goto LABEL_41;
      if ( v3 )
      {
        LocalFree(v3);
        v3 = 0i64;
        goto LABEL_23;
      }
    }
    v21 = ClRtlIsComputerObjectInDS(v13, (__int64)v11, v3, &v30, (wchar_t **)&hMem, 0i64);
    v9 = v21;
    if ( v21 /*signed*/< 0 )
    {
      v10 = uBytes;
      if ( (v21 & 0x1FFF0000) == 458752 )
        v9 = (unsigned __int16)v21;
      goto LABEL_42;
    }
    if ( !v30 )
    {
      v9 = 3;
LABEL_41:
      v10 = uBytes;
      goto LABEL_42;
    }
    v10 = uBytes;
    v22 = (BYTE *)LocalAlloc(0, (unsigned int)uBytes);
    v5 = (WCHAR *)v22;
    if ( v22 )
    {
      v24 = HIDWORD(uBytes);
      v9 = ClRtlGenerateRandomBytes(v22, SHIDWORD(uBytes));
      if ( v9 )
        goto LABEL_42;
      v9 = ClRtlKerbSetPassword(v42, v3, v5);
      if ( v9 )
        goto LABEL_42;
      if ( v31 )
      {
        memset_0(v41, 0, 0x384ui64);
        InBuffer = 0;
        StringCchCopyNW(v41, 0x80ui64, v5, v24);
        v23 = ClusterResourceControl(hResourcea, 0i64, 0x100031Au, &InBuffer, 0x388u, 0i64, 0, 0i64);
      }
      else
      {
        memset_0(v41, 0, 0x2A4ui64);
        InBuffer = 0;
        StringCchCopyNW(v41, 0x10ui64, v5, v24);
        v23 = ClusterResourceControl(hResourcea, 0i64, 0x100017Au, &InBuffer, 0x2A8u, 0i64, 0, 0i64);
      }
    }
    else
    {
      v23 = GetLastError();
    }
    v9 = v23;
LABEL_42:
    if ( v4 )
      LocalFree(v4);
    if ( hMem )
      LocalFree(hMem);
    if ( v3 )
      LocalFree(v3);
    v8 = v39;
    goto LABEL_6;
  }
  v9 = GetLastError();
LABEL_5:
  v10 = 0;
LABEL_6:
  if ( phDS )
    DsUnBindW(&phDS);
  if ( v5 )
  {
    memset(v5, 0, v10);
    LocalFree(v5);
  }
  if ( v6 )
    LocalFree(v6);
  if ( v8 )
    CloseCluster(v8);
  return v9;
}


==========

FUNCTION: ?ClRtlpDbgPrint@@YAXKPEAD0@Z @ 0x18001EE30
----------
void __fastcall ClRtlpDbgPrint(unsigned int a1, char *a2, char *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  Arguments = a3;
  if ( ClRtlpIsOutputDeviceAvailable() )
  {
    *(_QWORD *)&v5.Length = 0x4000000i64;
    DestinationString = 0i64;
    RtlInitAnsiString(&DestinationString, a2);
    v5.Buffer = (PWSTR)&v9;
    if ( RtlAnsiStringToUnicodeString(&v5, &DestinationString, 0) /*signed*/>= 0 )
    {
      v3 = FormatMessageW(0x400u, v5.Buffer, 0, 0, Buffer, 0x400u, &Arguments);
      if ( v3 )
        ClRtlpDbgPrint(Buffer, v3);
    }
  }
}


==========

FUNCTION: ?ClRtlpDbgPrint@@YAXPEA_W_K@Z @ 0x18001EF70
----------
void __fastcall ClRtlpDbgPrint(wchar_t *a1, unsigned __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = a2;
  if ( ClRtlpIsOutputDeviceAvailable() )
  {
    *(_DWORD *)(&DestinationString.MaximumLength + 1) = 0;
    *(_DWORD *)&DestinationString.Length = 67109888;
    SourceString.Buffer = a1;
    *(_QWORD *)&SourceString.Length = (unsigned __int16)(2 * v2);
    DestinationString.Buffer = OutputString;
    if ( RtlUnicodeStringToAnsiString(&DestinationString, &SourceString, 0) /*signed*/>= 0 )
      ClRtlpOutputString(OutputString);
  }
}


==========

FUNCTION: ?ClRtlpIsOutputDeviceAvailable@@YAHXZ @ 0x18001F01C
----------
int __fastcall ClRtlpIsOutputDeviceAvailable()
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v0 = 0;
  if ( ClRtlpDbgOutputToConsole || IsDebuggerPresent() )
    v0 = 1;
  return v0;
}


==========

FUNCTION: ?ClRtlpOutputString@@YAXPEAD@Z @ 0x18001F050
----------
void __fastcall ClRtlpOutputString(char *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(struct _RTL_CRITICAL_SECTION **)&lpCriticalSection;
  if ( !*(_QWORD *)&lpCriticalSection )
  {
    v3 = (struct _RTL_CRITICAL_SECTION *)LocalAlloc(lpCriticalSection, lpCriticalSection + 40);
    v4 = v3;
    if ( !v3 )
      return;
    InitializeCriticalSection(v3);
    _InterlockedCompareExchange64((volatile signed __int64 *)&lpCriticalSection, (signed __int64)v4, 0i64);
    v2 = *(struct _RTL_CRITICAL_SECTION **)&lpCriticalSection;
    if ( *(struct _RTL_CRITICAL_SECTION **)&lpCriticalSection == v4 )
    {
      qword_18003A4A8 = &lpCriticalSection;
    }
    else
    {
      DeleteCriticalSection(v4);
      LocalFree(v4);
      v2 = *(struct _RTL_CRITICAL_SECTION **)&lpCriticalSection;
    }
  }
  EnterCriticalSection(v2);
  if ( ClRtlpDbgOutputToConsole )
  {
    printf("%hs \n", a1);
  }
  else if ( IsDebuggerPresent() )
  {
    OutputDebugStringA(a1);
  }
  LeaveCriticalSection(*(LPCRITICAL_SECTION *)&lpCriticalSection);
}


==========

FUNCTION: ClRtlDbgPrint @ 0x18001F14C
----------
void ClRtlDbgPrint(__int64 a1, char *a2, ...)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  va_start(va, a2);
  ClRtlpDbgPrint(a1, a2, va);
}


==========

FUNCTION: ClRtlDupString @ 0x18001F174
----------
wchar_t *__fastcall ClRtlDupString(wchar_t *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = 0;
  v3 = -1i64;
  do
    ++v3;
  while ( a1[v3] );
  v4 = 2 * v3 + 2;
  v5 = (wchar_t *)LocalAlloc(0, v4);
  v6 = v5;
  if ( !v5 )
  {
    v2 = 8;
LABEL_7:
    LocalFree(v6);
    v6 = 0i64;
    goto LABEL_8;
  }
  v7 = StringCbCopyW(v5, v4, a1);
  if ( v7 /*signed*/< 0 )
  {
    v2 = (unsigned __int16)v7;
    if ( (_WORD)v7 )
      goto LABEL_7;
  }
LABEL_8:
  SetLastError(v2);
  return v6;
}


==========

FUNCTION: ClRtlLogPrint @ 0x18001F228
----------
NTSTATUS ClRtlLogPrint(unsigned int a1, const char *a2, ...)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  va_start(va1, a2);
  va_start(va, a2);
  v32 = va_arg(va1, _QWORD);
  v34 = va_arg(va1, _QWORD);
  *(_QWORD *)&v24.Length = 0x4000000i64;
  DestinationString = 0i64;
  *(_QWORD *)lpBuffer = 0i64;
  Arguments = 0i64;
  RtlInitAnsiString(&DestinationString, a2);
  v24.Buffer = Source;
  result = RtlAnsiStringToUnicodeString(&v24, &DestinationString, 0);
  if ( result /*signed*/< 0 )
    return result;
  v4 = -1i64;
  v5 = -1i64;
  do
    ++v5;
  while ( Source[v5] );
  if ( *((_WORD *)&v28 + v5 + 7) != 10 && v5 < 0x1FE )
  {
    *(_DWORD *)&Source[v5] = 655373;
    v6 = 2 * v5 + 4;
    if ( v6 >= 0x400 )
    {
      _report_rangecheckfailure(Source);
      __debugbreak();
    }
    *(_WORD *)((char *)Source + v6) = 0;
  }
  va_copy(Arguments, va);
  v7 = FormatMessageW(0x400u, Source, 0, 0, Buffer, 0x400u, &Arguments);
  if ( !v7 )
  {
    va_copy(Arguments, va);
    v7 = FormatMessageW(0x500u, Source, 0, 0, lpBuffer, 0x800u, &Arguments);
  }
  Arguments = 0i64;
  if ( v7 )
  {
    v22 = 0i64;
    v8 = (__int64 *)Buffer;
    if ( *(_QWORD *)lpBuffer )
      v8 = *(__int64 **)lpBuffer;
    v22 = v8;
    ClRtlpDbgPrint(Buffer, v7);
    switch ( a1 )
    {
      case 1u:
        v11 = CVERBOSECRITICAL;
        break;
      case 2u:
        v11 = CVERBOSEUNUSUAL;
        break;
      case 3u:
        v11 = CVERBOSENOISE;
        break;
      default:
        v11 = CVERBOSEDEBUG;
        break;
    }
    v26 = *(_OWORD *)v11;
    result = ClRtlpReportEvent(0, (__int64)&v26, v9, v10, &v22);
    if ( a1 > 4 )
      goto LABEL_34;
    v14 = a1 - 1;
    if ( v14 )
    {
      v15 = v14 - 1;
      if ( v15 )
      {
        if ( v15 == 1 )
          v16 = CNOISE;
        else
          v16 = CDEBUG;
      }
      else
      {
        v16 = CUNUSUAL;
      }
    }
    else
    {
      v16 = (__int64 *)&CCRITICAL;
    }
    v27 = *(_OWORD *)v16;
    v17 = &v22;
    v18 = &v27;
    v19 = 0;
  }
  else
  {
    StringCchPrintfW(v30, 0x200ui64, L"Couldn't format message: %ws\r\n", Buffer);
    do
      ++v4;
    while ( v30[v4] );
    ClRtlpDbgPrint(v30, v4);
    v23 = v30;
    v28 = CCRITICAL;
    v17 = (__int64 **)&v23;
    v18 = &v28;
    v19 = -1;
  }
  result = ClRtlpReportEvent(v19, (__int64)v18, v12, v13, v17);
LABEL_34:
  if ( *(_QWORD *)lpBuffer )
    result = (unsigned int)LocalFree(*(HLOCAL *)lpBuffer);
  return result;
}


==========

FUNCTION: ClRtlFindDwordProperty @ 0x18001F5B8
----------
__int64 __fastcall ClRtlFindDwordProperty(__int64 a1, __int64 a2, __int64 a3, _DWORD *a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v6[0] = 0i64;
  result = ClRtlFindDwordPropertyPtr((_DWORD *)a1, a2, (const WCHAR *)a3, v6);
  if ( (_DWORD)result )
    return result;
  if ( a4 )
    *a4 = *(_DWORD *)v6[0];
  return result;
}


==========

FUNCTION: ClRtlFindDwordPropertyPtr @ 0x18001F5F0
----------
__int64 __fastcall ClRtlFindDwordPropertyPtr(_DWORD *a1, unsigned int a2, const WCHAR *a3, _QWORD *a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a2 < 4 )
    return 13i64;
  v7 = *a1;
  v8 = (const WCHAR *)(a1 + 1);
  for ( i = a2 - 4; ; i -= 4 )
  {
    v10 = v7--;
    if ( !v10 || i /*signed*/<= 0 )
      return 2i64;
    if ( *(_DWORD *)v8 == 262147 && !lstrcmpiW(v8 + 4, a3) )
      break;
    do
    {
      v12 = (*((_DWORD *)v8 + 1) + 3) & 0xFFFFFFFC;
      i += -8 - v12;
      v8 = (const WCHAR *)((char *)v8 + v12 + 8);
    }
    while ( *(_DWORD *)v8 && i );
    v8 += 2;
  }
  v11 = (*((_DWORD *)v8 + 1) + 3) & 0xFFFFFFFC;
  if ( *(_DWORD *)((char *)v8 + v11 + 8) != 65538 )
  {
    ClRtlDbgPrint(1i64, "ClRtlFindDwordProperty: Property '%1!ls!' syntax (%2!d!, %3!d!) not proper list DWORD syntax.\n", a3, *(const WCHAR *)((char *)v8 + v11 + 10), *(const WCHAR *)((char *)v8 + v11 + 8));
    return 13i64;
  }
  if ( a4 )
    *a4 = (char *)v8 + v11 + 16;
  return 0i64;
}


==========

FUNCTION: ClRtlpFindSzProperty @ 0x18001F704
----------
__int64 __fastcall ClRtlpFindSzProperty(_DWORD *a1, unsigned int a2, __int64 a3, wchar_t **a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a2 < 4 )
    return 13i64;
  v6 = *a1;
  v7 = (const WCHAR *)(a1 + 1);
  v8 = a2 - 4;
  while ( 1 )
  {
    v9 = v6--;
    if ( !v9 || v8 /*signed*/<= 0 )
      return 2i64;
    if ( *(_DWORD *)v7 == 262147 && !lstrcmpiW(v7 + 4, L"CreatingDC") )
      break;
    v10 = -8 - ((*((_DWORD *)v7 + 1) + 3) & 0xFFFFFFFC) + v8;
    v11 = (*((_DWORD *)v7 + 1) + 3) & 0xFFFFFFFC;
    v12 = v7 + 4;
    while ( 1 )
    {
      v12 = (_DWORD *)((char *)v12 + v11);
      if ( !*v12 || !v10 )
        break;
      v11 = ((v12[1] + 3) & 0xFFFFFFFC) + 8;
      v10 -= v11;
    }
    v8 = v10 - 4;
    v7 = (const WCHAR *)(v12 + 1);
  }
  v13 = (*((_DWORD *)v7 + 1) + 3) & 0xFFFFFFFC;
  if ( (unsigned int)(*(_DWORD *)((char *)v7 + v13 + 8) - 65539) > 1 )
  {
    ClRtlDbgPrint(1i64, "ClRtlpFindSzProperty: Property '%1!ls!' syntax (%2!d!, %3!d!) not proper list string syntax.\n", L"CreatingDC", *(const WCHAR *)((char *)v7 + v13 + 10), *(const WCHAR *)((char *)v7 + v13 + 8));
    return 13i64;
  }
  if ( !a4 )
    return 0i64;
  v14 = *(_DWORD *)((char *)v7 + v13 + 12);
  v15 = (const WCHAR *)((char *)v7 + v13);
  if ( !v14 )
    v14 = 2;
  v16 = (wchar_t *)LocalAlloc(0, v14);
  v17 = v16;
  if ( !v16 )
    return 8i64;
  if ( v14 <= 2 )
    *v16 = 0;
  else
    StringCbCopyW(v16, v14, v15 + 8);
  *a4 = v17;
  return 0i64;
}


==========

FUNCTION: ClRtlpReportEvent @ 0x18001F874
----------
DWORD __fastcall ClRtlpReportEvent(__int16 a1, __int64 a2, __int64 a3, __int64 a4, __int64 **a5)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v6 = PublisherHandle;
  wcscpy((wchar_t *)&v14, L"NULL");
  if ( !PublisherHandle )
  {
    EnterCriticalSection(&EventLogLock);
    EtwEventRegister(MICROSOFT_FAILOVER_CLUSTER_PUBLISHER, 0i64, 0i64, &PublisherHandle);
    LeaveCriticalSection(&EventLogLock);
    v6 = PublisherHandle;
    if ( !PublisherHandle )
      return GetLastError();
  }
  result = EtwEventEnabled(v6, a2);
  if ( !(_BYTE)result )
    return result;
  if ( !*(_WORD *)(a2 + 6) )
    *(_WORD *)(a2 + 6) = a1;
  v9 = *a5;
  if ( *a5 )
  {
    v10 = -1i64;
    do
      ++v10;
    while ( *((_WORD *)v9 + v10) );
    v11 = 2 * v10 + 2;
  }
  else
  {
    v11 = 10;
    v9 = &v14;
  }
  v13[1] = v11;
  v13[0] = v9;
  v13[2] = &v12;
  v13[3] = 4i64;
  v13[4] = 0i64;
  v13[5] = 0i64;
  return EtwEventWrite(PublisherHandle, a2, 3i64, v13, 0);
}


==========

