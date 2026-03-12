Source file: Microsoft.FailoverClusters.FrameworkSupport
Generated time: 20260130_193942
==========

FUNCTION: DllMain @ 0x180008E10
----------
BOOL __stdcall DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return 1;
}


==========

FUNCTION: ??$ResetIterator@V?$ListHandle@VPropertyList@cxl@@@PropList@@@@YAKPEAV?$ListHandle@VPropertyList@cxl@@@PropList@@@Z @ 0x18000A7DC
----------
__int64 __fastcall ResetIterator<PropList::ListHandle<cxl::PropertyList>>(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *(_DWORD *)a1 != 1 )
    return 6i64;
  v2 = cxl::PropertyList::end((struct cxl::PropertyList *)(a1 + 8), (struct cxl::PropertyList::property_iterator *)v4);
  cxl::PropertyList::property_iterator::operator=(a1 + 48, (__int64)v2);
  cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v4);
  cxl::PropertyList::property_iterator::operator=(a1 + 192, a1 + 48);
  *(_BYTE *)(a1 + 336) = 0;
  return 0i64;
}


==========

FUNCTION: ??$ResetIterator@V?$ListHandle@VValueList@cxl@@@PropList@@@@YAKPEAV?$ListHandle@VValueList@cxl@@@PropList@@@Z @ 0x18000A878
----------
__int64 __fastcall ResetIterator<PropList::ListHandle<cxl::ValueList>>(_DWORD *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  PropList::ListHandle<cxl::ValueList>::reset((__int64)a1);
  return 0i64;
}


==========

FUNCTION: ?Close@?$DismPtr@II$1?DismCloseSession@@YAJI@Z@FrameworkSupport@@AEAAXXZ @ 0x18000CB00
----------
__int64 __fastcall FrameworkSupport::DismPtr<unsigned int,unsigned int,&long DismCloseSession(unsigned int)>::Close(_DWORD *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !*a1 )
    return result;
  result = DismCloseSession();
  *a1 = 0;
  return result;
}


==========

FUNCTION: ?Close@?$DismPtr@PEAU_DismFeatureInfo@@PEAX$1?DismDelete@@YAJPEAX@Z@FrameworkSupport@@AEAAXXZ @ 0x18000CB2C
----------
__int64 __fastcall FrameworkSupport::DismPtr<_DismFeatureInfo *,void *,&long DismDelete(void *)>::Close(_QWORD *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !*a1 )
    return result;
  result = DismDelete();
  *a1 = 0i64;
  return result;
}


==========

FUNCTION: ?GetErrorText@@YAJPEA_WPEAK@Z @ 0x18000CB7C
----------
int __fastcall GetErrorText(wchar_t *a1, unsigned int *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v10 = 0i64;
  FrameworkSupport::DismPtr<_DismFeatureInfo *,void *,&long DismDelete(void *)>::Close(&v10);
  v4 = DismGetLastErrorMessage(&v10);
  v5 = v4;
  if ( v4 /*signed*/>= 0 && v4 != 1 )
  {
    pcchLength = 0i64;
    if ( *v10 )
    {
      v5 = StringLengthWorkerW(*v10, 0x7FFFFFFFui64, &pcchLength);
      if ( v5 /*signed*/>= 0 )
      {
        v7 = pcchLength;
        v8 = *a2;
        if ( v8 >= 2 * pcchLength + 2 )
          v5 = StringCbCopyW(a1, v8, v6);
        else
          v5 = -2147024662;
        *a2 = 2 * v7 + 2;
      }
    }
    else
    {
      v5 = -2147024809;
    }
  }
  FrameworkSupport::DismPtr<_DismFeatureInfo *,void *,&long DismDelete(void *)>::Close(&v10);
  return v5;
}


==========

FUNCTION: CloseWindowsFeatures @ 0x18000E2A0
----------
__int64 __fastcall CloseWindowsFeatures(ServerManager::WindowsFeature *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 )
    return 87i64;
  v2 = a1;
  std::unique_ptr<ServerManager::WindowsFeature>::~unique_ptr<ServerManager::WindowsFeature>(&v2);
  return 0i64;
}


==========

FUNCTION: CreatePropList @ 0x18000E2D0
----------
char *__fastcall CreatePropList(struct CLUSPROP_LIST *propertyList, int propertyListSize)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = (char *)operator new(0x158ui64);
  *(_DWORD *)v4 = 1;
  cxl::PropertyList::PropertyList((struct cxl::PropertyList *)(v4 + 8));
  cxl::PropertyList::end((struct cxl::PropertyList *)(v4 + 8), (struct cxl::PropertyList::property_iterator *)(v4 + 48));
  cxl::PropertyList::end((struct cxl::PropertyList *)(v4 + 8), (struct cxl::PropertyList::property_iterator *)(v4 + 192));
  v5 = 0i64;
  v9 = (PropList::AutoList *)v4;
  v4[336] = 0;
  if ( !propertyList )
  {
    if ( !propertyListSize )
      goto LABEL_4;
    goto LABEL_7;
  }
  if ( !propertyListSize )
  {
LABEL_7:
    v6 = 87;
    goto LABEL_8;
  }
  v8[0] = propertyList;
  LODWORD(v8[1]) = propertyListSize;
  v6 = ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_b573ff8a43627b5b8bf6c7be37b87c4d___(v4, v8);
  if ( v6 )
  {
LABEL_8:
    SetLastError(v6);
    goto LABEL_5;
  }
LABEL_4:
  v9 = 0i64;
  v5 = v4;
LABEL_5:
  PropList::AutoList<cxl::PropertyList>::~AutoList<cxl::PropertyList>((void **)&v9);
  return v5;
}


==========

FUNCTION: CreateValueList @ 0x18000E3A0
----------
char *__fastcall CreateValueList(__int64 a1, int a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = operator new(0xC8ui64);
  *(_DWORD *)v4 = 1;
  cxl::ValueList::ValueList_void_((struct cxl::ValueList *)(v4 + 8));
  cxl::ValueList::end((__int64)(v4 + 8), (struct cxl::ValueList::value_iterator *)(v4 + 96));
  cxl::ValueList::end((__int64)(v4 + 8), (struct cxl::ValueList::value_iterator *)(v4 + 144));
  v5 = 0i64;
  v10 = v4;
  v4[192] = 0;
  if ( a1 )
  {
    if ( a2 )
      goto LABEL_3;
LABEL_6:
    v6 = 87;
    goto LABEL_4;
  }
  if ( a2 )
    goto LABEL_6;
LABEL_3:
  v8 = a1;
  v9 = a2;
  v6 = ExecuteOnList_PropList::ListHandle_cxl::ValueList___lambda_d968e976aea494ce79ca4695ff02f82d___(v4, (__int64)&v8);
  if ( !v6 )
  {
    v10 = 0i64;
    v5 = v4;
    goto LABEL_8;
  }
LABEL_4:
  SetLastError(v6);
LABEL_8:
  PropList::AutoList<cxl::ValueList>::~AutoList<cxl::ValueList>(&v10);
  return v5;
}


==========

FUNCTION: DestroyPropList @ 0x18000E470
----------
__int64 __fastcall DestroyPropList(HANDLE a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *(_DWORD *)a1 != 1 )
    return 6i64;
  PropList::ListHandle<cxl::PropertyList>::`scalar deleting destructor'(a1);
  return 0i64;
}


==========

FUNCTION: DestroyValueList @ 0x18000E4A0
----------
__int64 __fastcall DestroyValueList(_DWORD *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  PropList::ListHandle<cxl::ValueList>::`scalar deleting destructor'(a1);
  return 0i64;
}


==========

FUNCTION: GetClusterConnectionInformation @ 0x18000E500
----------
// attributes: thunk
__int64 __fastcall GetClusterConnectionInformation(__int64 a1, int a2, wchar_t *a3, unsigned int *a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return ClRtlGetClusterConnectionInformation(a1, a2, a3, a4);
}


==========

FUNCTION: GetInstalledWindowsFeatures @ 0x18000E550
----------
__int64 __fastcall GetInstalledWindowsFeatures(ServerManager::WindowsFeature *a1, int (__stdcall __high *a2)(unsigned int, enum _WindowsFeatrueLogLevel, unsigned int, const wchar_t *))
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a1 && a2 )
    result = ServerManager::WindowsFeature::GetInstalledFeatures(a1, a2);
  else
    result = 87i64;
  return result;
}


==========

FUNCTION: IsDismFeatureInstalled @ 0x18000E740
----------
__int64 __fastcall IsDismFeatureInstalled(__int64 a1, _DWORD *a2, wchar_t *a3, unsigned int *a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a2 = 0;
  if ( !a1 )
    return 2147942487i64;
  v11 = 0;
  FrameworkSupport::DismPtr<unsigned int,unsigned int,&long DismCloseSession(unsigned int)>::Close(&v11);
  v9 = DismOpenSession(L"DISM_{53BFAE52-B167-4E2F-A258-0A37B57FF845}", 0i64, 0i64, &v11);
  if ( v9 /*signed*/< 0 )
    goto LABEL_7;
  v12 = 0i64;
  FrameworkSupport::DismPtr<_DismFeatureInfo *,void *,&long DismDelete(void *)>::Close(&v12);
  v9 = DismGetFeatureInfo(v11, a1, 0i64, 1i64, &v12);
  if ( v9 /*signed*/>= 0 )
    *a2 = ((*(_DWORD *)(v12 + 8) - 4) & 0xFFFFFFFD) == 0;
  FrameworkSupport::DismPtr<_DismFeatureInfo *,void *,&long DismDelete(void *)>::Close(&v12);
  if ( v9 /*signed*/< 0 )
  {
LABEL_7:
    v10 = GetErrorText(a3, a4);
    if ( v10 /*signed*/< 0 )
      v9 = v10;
  }
  FrameworkSupport::DismPtr<unsigned int,unsigned int,&long DismCloseSession(unsigned int)>::Close(&v11);
  return (unsigned int)v9;
}


==========

FUNCTION: OpenWindowsFeatures @ 0x18000E840
----------
struct ServerManager::WindowsFeature *__fastcall OpenWindowsFeatures(_WORD *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  LODWORD(v4) = 0;
  if ( a1 && *a1 )
  {
    v5 = (struct ServerManager::WindowsFeature *)operator new(0xA8ui64);
    std::wstring::wstring(v6, a1);
    v2 = ServerManager::WindowsFeature::WindowsFeature(v5, (__int64)v6);
    std::wstring::_Tidy_deallocate((__int64)v6);
    v4 = 0i64;
    std::unique_ptr<ServerManager::WindowsFeature>::~unique_ptr<ServerManager::WindowsFeature>(&v4);
    result = v2;
  }
  else
  {
    SetLastError(0x57u);
    result = 0i64;
  }
  return result;
}


==========

FUNCTION: Reset @ 0x18000E910
----------
// attributes: thunk
__int64 __fastcall Reset(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return ResetIterator<PropList::ListHandle<cxl::PropertyList>>(a1);
}


==========

FUNCTION: ResetCnoPassword @ 0x18000E920
----------
// attributes: thunk
__int64 __fastcall ResetCnoPassword(struct _HRESOURCE *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return ClRtlResetPassword(a1, a2);
}


==========

FUNCTION: ResetValueList @ 0x18000E930
----------
// attributes: thunk
__int64 __fastcall ResetValueList(_DWORD *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return ResetIterator<PropList::ListHandle<cxl::ValueList>>(a1);
}


==========

FUNCTION: SetPropertyToDefaultValue @ 0x18000E940
----------
__int64 __fastcall SetPropertyToDefaultValue(_DWORD *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a2 )
    return 87i64;
  v3 = a2;
  return ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_17de830c8f2ff13b33ff664c5e949495___(a1, (__int64)&v3);
}


==========

FUNCTION: ?GetClusterFunctionalLevel@@YAKPEAU_HCLUSTER@@PEAK1@Z @ 0x18001DAA8
----------
unsigned int __fastcall GetClusterFunctionalLevel(struct _HCLUSTER *a1, unsigned int *a2, unsigned int *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  LODWORD(uBytes) = 0;
  result = ClusterControl(a1, 0i64, 0x7000055u, 0i64, 0, 0i64, 0, (LPDWORD)&uBytes);
  if ( result != 234 )
    return result;
  lpOutBuffer = LocalAlloc(0, (unsigned int)uBytes);
  v8 = ClusterControl(a1, 0i64, 0x7000055u, 0i64, 0, lpOutBuffer, uBytes, (LPDWORD)&uBytes);
  if ( !v8 && (!a2 || (v8 = ClRtlFindDwordProperty((__int64)lpOutBuffer, (unsigned int)uBytes, (__int64)L"ClusterFunctionalLevel", a2)) == 0) )
  {
    if ( a3 )
      v8 = ClRtlFindDwordProperty((__int64)lpOutBuffer, (unsigned int)uBytes, (__int64)L"ClusterUpgradeVersion", a3);
  }
  if ( lpOutBuffer )
    LocalFree(lpOutBuffer);
  return v8;
}


==========

FUNCTION: ??1CleanupdbgPrintLock@@QEAA@XZ @ 0x18001EDE8
----------
void __fastcall CleanupdbgPrintLock::~CleanupdbgPrintLock(struct CleanupdbgPrintLock *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(HLOCAL **)this;
  if ( v2 )
  {
    DeleteCriticalSection((LPCRITICAL_SECTION)*v2);
    LocalFree(**(HLOCAL **)this);
    **(_QWORD **)this = 0i64;
  }
}


==========

FUNCTION: MI_Instance_GetElement @ 0x180020598
----------
MI_Result __stdcall MI_Instance_GetElement(const MI_Instance *self, const MI_Char *name, MI_Value *value, MI_Type *type, MI_Uint32 *flags, MI_Uint32 *index)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( self && self->ft )
    result = ((unsigned int (__fastcall *)(const MI_Instance *, const MI_Char *, MI_Value *, MI_Type *, MI_Uint32 *, MI_Uint32 *))self->ft->GetElement)(self, name, value, type, flags, index);
  else
    result = MI_RESULT_INVALID_PARAMETER;
  return result;
}


==========

FUNCTION: MI_Operation_Cancel @ 0x1800227C0
----------
MI_Result __stdcall MI_Operation_Cancel(MI_Operation *operation, MI_CancellationReason reason)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( operation && (v2 = operation->ft) != 0i64 )
    result = ((unsigned int (__fastcall *)(MI_Operation *, _QWORD))v2->Cancel)(operation, 0i64);
  else
    result = MI_RESULT_INVALID_PARAMETER;
  return result;
}


==========

FUNCTION: MI_Session_Invoke @ 0x1800227E8
----------
void __stdcall MI_Session_Invoke(MI_Session *session, MI_Uint32 flags, MI_OperationOptions *options, const MI_Char *namespaceName, const MI_Char *className, const MI_Char *methodName, const MI_Instance *inboundInstance, const MI_Instance *inboundProperties, MI_OperationCallbacks *callbacks, MI_Operation *operation)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( session && (v10 = session->ft) != 0i64 )
  {
    ((void (__fastcall *)(MI_Session *, _QWORD, MI_OperationOptions *, const MI_Char *, const MI_Char *, const MI_Char *, const MI_Instance *, const MI_Instance *, MI_OperationCallbacks *, MI_Operation *))v10->Invoke)(session, 0i64, options, namespaceName, className, methodName, inboundInstance, inboundProperties, callbacks, operation);
  }
  else
  {
    if ( operation )
    {
      *(_OWORD *)&operation->reserved1 = 0i64;
      operation->ft = 0i64;
    }
    if ( callbacks )
    {
      v11 = callbacks->instanceResult;
      if ( v11 )
        ((void (__fastcall *)(_QWORD, void *, _QWORD, _QWORD, int, _QWORD, _QWORD, _QWORD))v11)(0i64, callbacks->callbackContext, 0i64, 0i64, 4, 0i64, 0i64, 0i64);
    }
  }
}


==========

FUNCTION: KerbSetPasswordUserEx @ 0x180022C74
----------
__int64 __fastcall KerbSetPasswordUserEx(PCWSTR SourceString, PCWSTR a2, PCWSTR a3, __int64 a4, PCWSTR SourceStringa)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v42 = HIDWORD(a4);
  LsaHandle = 0i64;
  AuthenticationPackage = 0;
  Buffer = 0i64;
  v6 = 0i64;
  ReturnBufferLength = 0;
  ProtocolStatus = 0;
  v9 = 11;
  v40 = 0i64;
  v37 = 0i64;
  v36 = 0i64;
  v33 = 0i64;
  DestinationString = 0i64;
  v39 = 0i64;
  v38 = 0i64;
  if ( SourceStringa )
    v9 = 12;
  RtlInitUnicodeString(&DestinationString, SourceStringa);
  v10 = LsaConnectUntrusted(&LsaHandle);
  if ( v10 /*signed*/>= 0 )
  {
    RtlInitString(&v40, "Kerberos");
    v10 = LsaLookupAuthenticationPackage(LsaHandle, (PLSA_STRING)&v40, &AuthenticationPackage);
    if ( v10 /*signed*/>= 0 )
    {
      RtlInitUnicodeString(&v37, a2);
      RtlInitUnicodeString(&v36, SourceString);
      RtlInitUnicodeString(&v33, a3);
      RtlInitUnicodeString(&v39, 0i64);
      RtlInitUnicodeString(&v38, 0i64);
      if ( v33.Length <= 0xFEu )
      {
        v11 = v37.Length + v36.Length + DestinationString.Length + v39.Length + v38.Length + v33.Length + 152;
        v12 = (char *)LocalAlloc(0x40u, v11);
        v6 = (unsigned __int16 *)v12;
        if ( v12 )
        {
          *((_DWORD *)v12 + 36) = 0;
          *(_DWORD *)v12 = v9;
          v13 = (void *)((unsigned __int64)(v12 + 155) & 0xFFFFFFFFFFFFFFFCui64);
          *(struct _UNICODE_STRING *)(v12 + 40) = v36;
          *((_QWORD *)v12 + 6) = v13;
          memcpy_0(v13, v36.Buffer, v36.Length);
          v14 = v6[20];
          v15 = *((_QWORD *)v6 + 6);
          *(struct _UNICODE_STRING *)(v6 + 28) = v37;
          v16 = (void *)(v15 + 2 * (v14 >> 1));
          *((_QWORD *)v6 + 8) = v16;
          memcpy_0(v16, v37.Buffer, v37.Length);
          v17 = v6[28];
          v18 = *((_QWORD *)v6 + 8);
          *(struct _UNICODE_STRING *)(v6 + 36) = v33;
          v19 = (void *)(v18 + 2 * (v17 >> 1));
          *((_QWORD *)v6 + 10) = v19;
          memcpy_0(v19, v33.Buffer, v33.Length);
          v20 = v6[36];
          v21 = *((_QWORD *)v6 + 10);
          *(struct _UNICODE_STRING *)(v6 + 44) = v38;
          v22 = (void *)(v21 + 2 * (v20 >> 1));
          *((_QWORD *)v6 + 12) = v22;
          memcpy_0(v22, v38.Buffer, v38.Length);
          v23 = v6[44];
          v24 = *((_QWORD *)v6 + 12);
          *(struct _UNICODE_STRING *)(v6 + 52) = v39;
          v25 = (void *)(v24 + 2 * (v23 >> 1));
          *((_QWORD *)v6 + 14) = v25;
          memcpy_0(v25, v39.Buffer, v39.Length);
          v26 = v6[44];
          v27 = *((_QWORD *)v6 + 12);
          *((struct _UNICODE_STRING *)v6 + 8) = DestinationString;
          v28 = (void *)(v27 + 2 * (v26 >> 1));
          *((_QWORD *)v6 + 17) = v28;
          memcpy_0(v28, DestinationString.Buffer, DestinationString.Length);
          v10 = LsaCallAuthenticationPackage(LsaHandle, AuthenticationPackage, v6, v11, &Buffer, &ReturnBufferLength, &ProtocolStatus);
          if ( v10 /*signed*/>= 0 && ProtocolStatus /*signed*/< 0 )
            v10 = ProtocolStatus;
        }
      }
      else
      {
        v10 = -1073741562;
      }
    }
  }
  if ( LsaHandle )
    LsaDeregisterLogonProcess(LsaHandle);
  if ( Buffer )
    LsaFreeReturnBuffer(Buffer);
  if ( v6 )
    LocalFree(v6);
  return (unsigned int)v10;
}


==========

