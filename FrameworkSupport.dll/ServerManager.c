Source file: Microsoft.FailoverClusters.FrameworkSupport
Generated time: 20260130_193835
==========

FUNCTION: ??0WindowsFeature@ServerManager@@QEAA@AEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@Z @ 0x18000FCD8
----------
struct ServerManager::WindowsFeature *__fastcall ServerManager::WindowsFeature::WindowsFeature(struct ServerManager::WindowsFeature *this, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::wstring::wstring(this, a2);
  std::wstring::wstring(v4, L"82FDCD5F-2D62-4B95-BA8F-2A73A1D4AAB6");
  mi::MiApplication::Create((__int64 *)this + 4, (__int64)v4);
  std::wstring::_Tidy_deallocate((__int64)v4);
  *((_QWORD *)this + 6) = 0i64;
  *((_QWORD *)this + 7) = 0i64;
  mi::MiAsyncOperation::MiAsyncOperation((struct ServerManager::WindowsFeature *)((char *)this + 64));
  InitializeSRWLock((PSRWLOCK)this + 18);
  *((_DWORD *)this + 38) = 0;
  *((_DWORD *)this + 39) = 0;
  *((_BYTE *)this + 160) = 0;
  return this;
}


==========

FUNCTION: ??1WindowsFeature@ServerManager@@QEAA@XZ @ 0x180010178
----------
void __fastcall ServerManager::WindowsFeature::~WindowsFeature(struct ServerManager::WindowsFeature *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v1 = (struct cxl::NonReentrantRWLock *)((char *)this + 144);
  cxl::AcquireWriteLock::AcquireWriteLock(&v7, (struct cxl::NonReentrantRWLock *)this + 9);
  v4 = *((_QWORD *)this + 15) == 0i64;
  *((_BYTE *)this + 160) = 1;
  if ( !v4 )
    mi::MiOperation::Cancel((struct mi::MiOperation *)this + 1, v3);
  cxl::ScopedResOwner<cxl::NonReentrantRWLock,cxl::AcquireExclusiveTraits<cxl::NonReentrantRWLock>>::Release((__int64)&v7);
  cxl::NonReentrantRWLock::~NonReentrantRWLock(v1);
  mi::MiAsyncOperation::~MiAsyncOperation((struct ServerManager::WindowsFeature *)((char *)this + 64));
  v5 = (std::_Ref_count_base *)*((_QWORD *)this + 7);
  if ( v5 )
    std::_Ref_count_base::_Decref(v5);
  v6 = (std::_Ref_count_base *)*((_QWORD *)this + 5);
  if ( v6 )
    std::_Ref_count_base::_Decref(v6);
  std::wstring::_Tidy_deallocate((__int64)this);
}


==========

FUNCTION: ??RResultsFunctor@ServerManager@@QEBAXAEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEBVMiValue@mi@@@Z @ 0x180010330
----------
void __fastcall ServerManager::ResultsFunctor::operator()(struct ServerManager::ResultsFunctor *this, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = v2;
  std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>(v24);
  v6 = (const WCHAR *)std::_String_val<std::_Simple_types<wchar_t>>::_Myptr(a2);
  if ( CompareStringW(0x400u, 1u, v6, -1, L"ServerComponents", -1) != 2 )
  {
    v7 = std::operator<<<wchar_t,std::char_traits<wchar_t>>((__int64)v25, (__int64)L"Unknown object '");
    v8 = std::operator<<<wchar_t>(v7, a2);
    v9 = std::operator<<<wchar_t,std::char_traits<wchar_t>>(v8, (__int64)L"' received from ");
    v10 = std::operator<<<wchar_t,std::char_traits<wchar_t>>(v9, (__int64)L"MSFT_ServerManagerDeploymentTasks");
    v11 = std::operator<<<wchar_t,std::char_traits<wchar_t>>(v10, (__int64)L"::");
    std::operator<<<wchar_t,std::char_traits<wchar_t>>(v11, (__int64)L"GetServerComponentsAsync");
    v12 = *(void (__fastcall **)(_QWORD, __int64, _QWORD, _QWORD *))this;
    v13 = std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::str((__int64)v24, (__int64)v26);
    v14 = std::_String_val<std::_Simple_types<wchar_t>>::_Myptr(v13);
    v12(0i64, 3i64, 0i64, v14);
LABEL_5:
    std::wstring::_Tidy_deallocate((__int64)v26);
    goto LABEL_8;
  }
  if ( *(_DWORD *)(v3 + 80) != 31 )
  {
    v15 = std::operator<<<wchar_t,std::char_traits<wchar_t>>((__int64)v25, (__int64)L"MSFT_ServerManagerDeploymentTasks");
    v16 = std::operator<<<wchar_t,std::char_traits<wchar_t>>(v15, (__int64)L"::");
    v17 = std::operator<<<wchar_t,std::char_traits<wchar_t>>(v16, (__int64)L"GetServerComponentsAsync");
    v18 = std::operator<<<wchar_t,std::char_traits<wchar_t>>(v17, (__int64)L" returned an unknown result type.  Type: ");
    v19 = std::wostream::operator<<(v18, *(unsigned int *)(v3 + 80));
    std::operator<<<wchar_t,std::char_traits<wchar_t>>(v19, (__int64)L" received");
    v20 = *(void (__fastcall **)(_QWORD, __int64, _QWORD, _QWORD *))this;
    v21 = std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::str((__int64)v24, (__int64)v26);
    v22 = std::_String_val<std::_Simple_types<wchar_t>>::_Myptr(v21);
    v20(0i64, 3i64, 0i64, v22);
    goto LABEL_5;
  }
  *(_OWORD *)v26 = 0i64;
  v27 = 0i64;
  mi::MiValue::GetValue<std::vector<mi::MiInstance>>(v3, v26);
  ServerManager::ResultsFunctor::ProcessResults(this, (__int64)v26, (__int64)v24);
  v23 = v26[0];
  if ( v26[0] )
  {
    std::_Destroy_range<std::allocator<mi::MiInstance>>(v26[0], v26[1]);
    std::_Deallocate<16,0>(v23, 8 * ((v27 - (__int64)v23) /*signed*/>> 3));
  }
LABEL_8:
  std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::`vbase destructor((__int64)v24);
}


==========

FUNCTION: ?GetInstalledFeatures@WindowsFeature@ServerManager@@QEAAKP6AHKW4_WindowsFeatrueLogLevel@@KPEB_W@Z@Z @ 0x1800106C4
----------
__int64 __fastcall ServerManager::WindowsFeature::GetInstalledFeatures(ServerManager::WindowsFeature *this, int (__stdcall __high *a2)(unsigned int, enum _WindowsFeatrueLogLevel, unsigned int, const wchar_t *))
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = *((_QWORD *)this + 4);
  std::wstring::wstring(v20, L"root\\Microsoft\\Windows\\ServerManager");
  v5 = mi::MiApplication::CreateSession(v4, &v16, (__int64)v20, (__int64)this);
  v6 = *v5;
  v7 = v5[1];
  *v5 = 0i64;
  v5[1] = 0i64;
  *((_QWORD *)this + 6) = v6;
  v8 = (std::_Ref_count_base *)*((_QWORD *)this + 7);
  *((_QWORD *)this + 7) = v7;
  if ( v8 )
    std::_Ref_count_base::_Decref(v8);
  if ( v16.lock_ptr_08 )
    std::_Ref_count_base::_Decref(v16.lock_ptr_08);
  std::wstring::_Tidy_deallocate((__int64)v20);
  mi::MiApplication::CreateParameterSet(*((_QWORD *)this + 4), &v24);
  v9 = (MI_Uint64 *)ServerManager::WindowsFeature::get_RequestGuid((__int64)this, (__int64)&v21);
  std::wstring::wstring(v20, L"RequestGuid");
  *(_OWORD *)(&v19.boolean + 1) = 0i64;
  *(MI_BooleanA *)((char *)&v19.booleana + 17) = 0i64;
  *(_DWORD *)((char *)&v19.array + 33) = 0;
  *(_WORD *)((char *)&v19.array + 37) = 0;
  *((_BYTE *)&v19.array + 39) = 0;
  v19.uint64 = *v9;
  mi::MiInstance::AddElement(&v24, (__int64)v20, (enum _MI_Type)15, &v19, 0);
  std::wstring::_Tidy_deallocate((__int64)v20);
  mi::MiInstance::~MiInstance(&v21);
  v10 = *((_QWORD *)this + 6);
  v23[0] = (__int64)&std::_Func_impl_no_alloc<ServerManager::ResultsFunctor,void,std::wstring const &,mi::MiValue const &>::`vftable';
  v23[1] = (__int64)a2;
  v23[7] = (__int64)v23;
  *(_QWORD *)v16.padding_00_07 = this;
  v16.lock_ptr_08 = (struct cxl_NonReentrantRWLock *)a2;
  v17 = &v24;
  *(_QWORD *)v21.mi_object_00 = &std::_Func_impl_no_alloc<_lambda_676d3e6bbfe2e4dd683b81c46d39e3aa_,bool,mi::MiInstance const &,enum _MI_Result,std::wstring const &,mi::MiInstance const &>::`vftable';
  *(struct cxl::AcquireWriteLock *)&v21.shared_ptr_08 = v16;
  *(_QWORD *)v21.padding_18_27 = &v24;
  v22 = &v21;
  std::wstring::wstring(&v19, L"GetServerComponentsAsync");
  std::wstring::wstring(v20, L"MSFT_ServerManagerDeploymentTasks");
  v11 = mi::MiSession::InvokeMethod(v10, (__int64)v18, (__int64)v20, (__int64)&v19, (const MI_Instance **)&v24, (__int64)&v21, (__int64)v23);
  mi::MiAsyncOperation::operator=((__int64)this + 64, v11);
  mi::MiAsyncOperation::~MiAsyncOperation((struct mi::MiAsyncOperation *)v18);
  std::wstring::_Tidy_deallocate((__int64)v20);
  std::wstring::_Tidy_deallocate((__int64)&v19);
  std::function<void (CLUSPROP_ULARGE_INTEGER *)>::~function<void (CLUSPROP_ULARGE_INTEGER *)>((__int64)&v21, v12);
  std::function<void (CLUSPROP_ULARGE_INTEGER *)>::~function<void (CLUSPROP_ULARGE_INTEGER *)>((__int64)v23, v13);
  mi::MiAsyncOperation::Join((ServerManager::WindowsFeature *)((char *)this + 64));
  cxl::AcquireWriteLock::AcquireWriteLock(&v16, (struct cxl::NonReentrantRWLock *)this + 9);
  v14 = mi::MiAsyncOperation::MiAsyncOperation((struct mi::MiAsyncOperation *)v18);
  mi::MiAsyncOperation::operator=((__int64)this + 64, (__int64)v14);
  mi::MiAsyncOperation::~MiAsyncOperation((struct mi::MiAsyncOperation *)v18);
  *((_BYTE *)this + 160) = 1;
  cxl::ScopedResOwner<cxl::NonReentrantRWLock,cxl::AcquireExclusiveTraits<cxl::NonReentrantRWLock>>::Release((__int64)&v16);
  mi::MiInstance::~MiInstance(&v24);
  return 0i64;
}


==========

FUNCTION: ?HandleInProgress@WindowsFeature@ServerManager@@AEAAXAEBVMiInstance@mi@@P6AHKW4_WindowsFeatrueLogLevel@@KPEB_W@Z@Z @ 0x18001094C
----------
void __fastcall ServerManager::WindowsFeature::HandleInProgress(ServerManager::WindowsFeature *this, const struct mi::MiInstance *a2, int (__stdcall __high *a3)(unsigned int, enum _WindowsFeatrueLogLevel, unsigned int, const wchar_t *))
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_OWORD *)v13.mi_object_00 = 0i64;
  *(_OWORD *)v13.padding_10_0 = 0i64;
  *(_OWORD *)&v13.padding_18_27[8] = 0i64;
  LOBYTE(v13.vtable_30) = 1;
  v6 = *((_QWORD *)this + 6);
  v20[0] = (__int64)&std::_Func_impl_no_alloc<ServerManager::ResultsFunctor,void,std::wstring const &,mi::MiValue const &>::`vftable';
  v20[1] = (__int64)a3;
  v20[7] = (__int64)v20;
  *(_QWORD *)&v10 = this;
  *((_QWORD *)&v10 + 1) = a3;
  *(_QWORD *)&v11 = a2;
  *((_QWORD *)&v11 + 1) = &v13;
  v14 = &std::_Func_impl_no_alloc<_lambda_b1d2c8a700cdba81b94ab795b53c7a01_,bool,mi::MiInstance const &,enum _MI_Result,std::wstring const &,mi::MiInstance const &>::`vftable';
  v15 = v10;
  v16 = v11;
  v17 = &v14;
  std::wstring::wstring(v19, L"GetEnumerationRequestState");
  std::wstring::wstring(v18, L"MSFT_ServerManagerDeploymentTasks");
  v7 = (struct mi::MiAsyncOperation *)mi::MiSession::InvokeMethod(v6, (__int64)v12, (__int64)v18, (__int64)v19, (const MI_Instance **)a2, (__int64)&v14, (__int64)v20);
  mi::MiAsyncOperation::Join(v7);
  mi::MiAsyncOperation::~MiAsyncOperation((struct mi::MiAsyncOperation *)v12);
  std::wstring::_Tidy_deallocate((__int64)v18);
  std::wstring::_Tidy_deallocate((__int64)v19);
  std::function<void (CLUSPROP_ULARGE_INTEGER *)>::~function<void (CLUSPROP_ULARGE_INTEGER *)>((__int64)&v14, v8);
  std::function<void (CLUSPROP_ULARGE_INTEGER *)>::~function<void (CLUSPROP_ULARGE_INTEGER *)>((__int64)v20, v9);
  ServerManager::WindowsFeature::ProcessMethodResults(this, &v13, a2, a3);
  mi::MiInstance::~MiInstance(&v13);
}


==========

FUNCTION: ?ProcessEnumState@WindowsFeature@ServerManager@@AEAAXAEBVMiProperty@mi@@AEBVMiInstance@4@P6AHKW4_WindowsFeatrueLogLevel@@KPEB_W@Z@Z @ 0x180010ABC
----------
void __fastcall ServerManager::WindowsFeature::ProcessEnumState(ServerManager::WindowsFeature *this, const struct mi::MiProperty *a2, const struct mi::MiInstance *a3, int (__stdcall __high *a4)(unsigned int, enum _WindowsFeatrueLogLevel, unsigned int, const wchar_t *))
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>(v34);
  v31[0] = 0;
  v32 = 0i64;
  v33 = 0i64;
  *(_OWORD *)v40.mi_object_00 = 0i64;
  *(_OWORD *)v40.padding_10_0 = 0i64;
  *(_OWORD *)&v40.padding_18_27[8] = 0i64;
  LOBYTE(v40.vtable_30) = 1;
  mi::MiValue::GetValue<mi::MiInstance>((struct mi *)a2, &v40);
  std::wstring::wstring(&v36, L"RequestState");
  v8 = (struct mi::MiValue *)mi::MiInstance::operator[]((__int64)&v40, (struct mi::MiProperty *)&v42, (__int64)&v36);
  mi::MiValue::GetValue<unsigned char>(v8, (unsigned __int8 *)v31);
  *(_QWORD *)v42.padding_00_07 = &mi::MiValue::`vftable';
  std::wstring::_Tidy_deallocate((__int64)&v42.field_08);
  std::wstring::_Tidy_deallocate((__int64)&v36);
  std::wstring::wstring(&v36, L"Warnings");
  v9 = mi::MiInstance::operator[]((__int64)&v40, (struct mi::MiProperty *)&v42, (__int64)&v36);
  mi::MiValue::GetValue<std::vector<std::wstring>>((__int64)v9, (__int64)&v32);
  *(_QWORD *)v42.padding_00_07 = &mi::MiValue::`vftable';
  std::wstring::_Tidy_deallocate((__int64)&v42.field_08);
  std::wstring::_Tidy_deallocate((__int64)&v36);
  v10 = (unsigned __int8)v31[0];
  if ( v31[0] == 2 )
  {
    v36 = 0i64;
    v37 = 0i64;
    v38 = 7i64;
    LOWORD(v36) = 0;
    std::wstring::wstring(v43, L"Error");
    mi::MiInstance::operator[]((__int64)&v40, (struct mi::MiProperty *)&v42, (__int64)v43);
    std::wstring::_Tidy_deallocate((__int64)v43);
    if ( (HIDWORD(v42.field_50) & 0x20000000) == 0 )
    {
      *(_OWORD *)v41.mi_object_00 = 0i64;
      *(_OWORD *)v41.padding_10_0 = 0i64;
      *(_OWORD *)&v41.padding_18_27[8] = 0i64;
      LOBYTE(v41.vtable_30) = 1;
      mi::MiValue::GetValue<mi::MiInstance>(&v42, &v41);
      std::wstring::wstring(v39, L"Message");
      v11 = mi::MiInstance::operator[]((__int64)&v41, (struct mi::MiProperty *)&v44, (__int64)v39);
      mi::MiValue::get_value<std::wstring>((__int64)v11, (__int64)&v36);
      v44 = &mi::MiValue::`vftable';
      std::wstring::_Tidy_deallocate((__int64)v45);
      std::wstring::_Tidy_deallocate((__int64)v39);
      mi::MiInstance::~MiInstance(&v41);
    }
    if ( v37 )
      v12 = (const wchar_t *)std::_String_val<std::_Simple_types<wchar_t>>::_Myptr((__int64)&v36);
    else
      v12 = L"<Failed to get error message>";
    ((void (__fastcall *)(_QWORD, __int64, __int64, const wchar_t *))a4)(0i64, 3i64, 1i64, v12);
    *(_QWORD *)v42.padding_00_07 = &mi::MiValue::`vftable';
    std::wstring::_Tidy_deallocate((__int64)&v42.field_08);
    v13 = &v36;
  }
  else
  {
    if ( !v31[0] )
    {
      Sleep(0xFAu);
      ServerManager::WindowsFeature::HandleInProgress(this, a3, a4);
      goto LABEL_15;
    }
    v14 = std::operator<<<wchar_t,std::char_traits<wchar_t>>((__int64)v35, (__int64)L"MSFT_ServerManagerDeploymentTasks");
    v15 = std::operator<<<wchar_t,std::char_traits<wchar_t>>(v14, (__int64)L"::");
    v16 = std::operator<<<wchar_t,std::char_traits<wchar_t>>(v15, (__int64)L"GetServerComponentsAsync");
    if ( (_BYTE)v10 == 1 )
    {
      std::operator<<<wchar_t,std::char_traits<wchar_t>>(v16, (__int64)L" completed successfully");
      v20 = std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::str((__int64)v34, (__int64)v39);
      v21 = std::_String_val<std::_Simple_types<wchar_t>>::_Myptr(v20);
      ((void (__fastcall *)(_QWORD, __int64, _QWORD, _QWORD *))a4)(0i64, 1i64, 0i64, v21);
    }
    else
    {
      v17 = std::operator<<<wchar_t,std::char_traits<wchar_t>>(v16, (__int64)L" is not complete.  Current state is: ");
      std::wostream::operator<<(v17, v10);
      v18 = std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::str((__int64)v34, (__int64)v39);
      v19 = std::_String_val<std::_Simple_types<wchar_t>>::_Myptr(v18);
      ((void (__fastcall *)(_QWORD, __int64, _QWORD, _QWORD *))a4)(0i64, 2i64, 0i64, v19);
    }
    v13 = (__int128 *)v39;
  }
  std::wstring::_Tidy_deallocate((__int64)v13);
LABEL_15:
  v22 = *((_QWORD *)&v32 + 1);
  v23 = v32;
  if ( (__int64)(*((_QWORD *)&v32 + 1) - v32) /*signed*/>> 5 )
  {
    v24 = std::operator<<<wchar_t,std::char_traits<wchar_t>>((__int64)v35, (__int64)L"MSFT_ServerManagerDeploymentTasks");
    v25 = std::operator<<<wchar_t,std::char_traits<wchar_t>>(v24, (__int64)L"::");
    v26 = std::operator<<<wchar_t,std::char_traits<wchar_t>>(v25, (__int64)L"GetServerComponentsAsync");
    v27 = std::operator<<<wchar_t,std::char_traits<wchar_t>>(v26, (__int64)L" Returned warnings:");
    std::wostream::operator<<(v27, std::endl<wchar_t,std::char_traits<wchar_t>>);
    while ( v23 != v22 )
    {
      v28 = std::operator<<<wchar_t>((__int64)v35, v23);
      std::wostream::operator<<(v28, std::endl<wchar_t,std::char_traits<wchar_t>>);
      v23 += 32i64;
    }
    v29 = std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::str((__int64)v34, (__int64)v39);
    v30 = std::_String_val<std::_Simple_types<wchar_t>>::_Myptr(v29);
    ((void (__fastcall *)(_QWORD, __int64, _QWORD, _QWORD *))a4)(0i64, 2i64, 0i64, v30);
    std::wstring::_Tidy_deallocate((__int64)v39);
  }
  mi::MiInstance::~MiInstance(&v40);
  std::vector<std::wstring>::_Tidy((__int64)&v32);
  std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::`vbase destructor((__int64)v34);
}


==========

FUNCTION: ?ProcessMethodResults@WindowsFeature@ServerManager@@AEAAXAEBVMiInstance@mi@@0P6AHKW4_WindowsFeatrueLogLevel@@KPEB_W@Z@Z @ 0x180010F54
----------
void __fastcall ServerManager::WindowsFeature::ProcessMethodResults(ServerManager::WindowsFeature *this, const struct mi::MiInstance *a2, const struct mi::MiInstance *a3, int (__stdcall __high *a4)(unsigned int, enum _WindowsFeatrueLogLevel, unsigned int, const wchar_t *))
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>(v27);
  std::wstring::wstring(v29, L"ReturnValue");
  v8 = *((_DWORD *)mi::MiInstance::operator[]((__int64)a2, (struct mi::MiProperty *)&v31, (__int64)v29) + 21);
  v31 = &mi::MiValue::`vftable';
  std::wstring::_Tidy_deallocate((__int64)v32);
  std::wstring::_Tidy_deallocate((__int64)v29);
  if ( _bittest(&v8, 0x1Du) )
  {
    v21 = std::operator<<<wchar_t,std::char_traits<wchar_t>>((__int64)v28, (__int64)L"MSFT_ServerManagerDeploymentTasks");
    v22 = std::operator<<<wchar_t,std::char_traits<wchar_t>>(v21, (__int64)L"::");
    v23 = std::operator<<<wchar_t,std::char_traits<wchar_t>>(v22, (__int64)L"GetServerComponentsAsync");
    std::operator<<<wchar_t,std::char_traits<wchar_t>>(v23, (__int64)L" ReturnValue is NULL.");
    v24 = std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::str((__int64)v27, (__int64)v30);
    v25 = std::_String_val<std::_Simple_types<wchar_t>>::_Myptr(v24);
    ((void (__fastcall *)(_QWORD, __int64, __int64, _QWORD *))a4)(0i64, 3i64, 1i64, v25);
    v17 = v30;
  }
  else
  {
    v26[0] = 0;
    std::wstring::wstring(v29, L"ReturnValue");
    v9 = (struct mi::MiValue *)mi::MiInstance::operator[]((__int64)a2, (struct mi::MiProperty *)&v31, (__int64)v29);
    mi::MiValue::GetValue<unsigned long>(v9, v26);
    v31 = &mi::MiValue::`vftable';
    std::wstring::_Tidy_deallocate((__int64)v32);
    std::wstring::_Tidy_deallocate((__int64)v29);
    v10 = v26[0];
    if ( v26[0] )
    {
      v11 = std::operator<<<wchar_t,std::char_traits<wchar_t>>((__int64)v28, (__int64)L"MSFT_ServerManagerDeploymentTasks");
      v12 = std::operator<<<wchar_t,std::char_traits<wchar_t>>(v11, (__int64)L"::");
      v13 = std::operator<<<wchar_t,std::char_traits<wchar_t>>(v12, (__int64)L"GetServerComponentsAsync");
      v14 = std::operator<<<wchar_t,std::char_traits<wchar_t>>(v13, (__int64)L" Returned: ");
      std::wostream::operator<<(v14, v10);
      v15 = std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::str((__int64)v27, (__int64)v29);
      v16 = std::_String_val<std::_Simple_types<wchar_t>>::_Myptr(v15);
      ((void (__fastcall *)(_QWORD, __int64, _QWORD, _QWORD *))a4)(0i64, 3i64, v10, v16);
      v17 = (char *)v29;
    }
    else
    {
      std::wstring::wstring(v29, L"EnumerationState");
      mi::MiInstance::operator[]((__int64)a2, (struct mi::MiProperty *)&v31, (__int64)v29);
      std::wstring::_Tidy_deallocate((__int64)v29);
      if ( (v33 & 0x20000000) != 0 )
      {
        v18 = std::operator<<<wchar_t,std::char_traits<wchar_t>>((__int64)v28, (__int64)L"EnumerationState");
        std::operator<<<wchar_t,std::char_traits<wchar_t>>(v18, (__int64)L" is NULL");
        v19 = std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::str((__int64)v27, (__int64)v30);
        v20 = std::_String_val<std::_Simple_types<wchar_t>>::_Myptr(v19);
        ((void (__fastcall *)(_QWORD, __int64, __int64, _QWORD *))a4)(0i64, 3i64, 1i64, v20);
        std::wstring::_Tidy_deallocate((__int64)v30);
      }
      else
      {
        ServerManager::WindowsFeature::ProcessEnumState(this, (const struct mi::MiProperty *)&v31, a3, a4);
      }
      v31 = &mi::MiValue::`vftable';
      v17 = v32;
    }
  }
  std::wstring::_Tidy_deallocate((__int64)v17);
  std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::`vbase destructor((__int64)v27);
}


==========

FUNCTION: ?ProcessResults@ResultsFunctor@ServerManager@@AEBAXAEBV?$vector@VMiInstance@mi@@V?$allocator@VMiInstance@mi@@@std@@@std@@AEAV?$basic_stringstream@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@4@@Z @ 0x180011234
----------
void __fastcall ServerManager::ResultsFunctor::ProcessResults(struct ServerManager::ResultsFunctor *this, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v5 = *(_QWORD *)a2;
  v6 = *(_QWORD *)(a2 + 8);
  if ( *(_QWORD *)a2 != v6 )
  {
    do
    {
      v7 = 0;
      v13 = 0;
      v12[0] = 0;
      std::wstring::wstring(v14, L"NumericId");
      mi::MiInstance::operator[](v5, (struct mi::MiProperty *)&v16, (__int64)v14);
      std::wstring::_Tidy_deallocate((__int64)v14);
      std::wstring::wstring(v15, L"Installed");
      mi::MiInstance::operator[](v5, (struct mi::MiProperty *)&v18, (__int64)v15);
      std::wstring::_Tidy_deallocate((__int64)v15);
      if ( (v17 & 0x20000000) != 0 || (v19 & 0x20000000) != 0 )
      {
        if ( (v17 & 0x20000000) != 0 )
        {
          v8 = L"Unknown feature Id received";
        }
        else
        {
          mi::MiValue::GetValue<int>(&v16, &v13);
          v8 = L"Unable to determine if feature is installed.";
          v7 = v13;
        }
        std::operator<<<wchar_t,std::char_traits<wchar_t>>(a3 + 16, (__int64)v8);
        v9 = *(void (__fastcall **)(_QWORD, __int64, _QWORD, _QWORD *))this;
        v10 = std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::str(a3, (__int64)v14);
        v11 = std::_String_val<std::_Simple_types<wchar_t>>::_Myptr(v10);
        v9(v7, 3i64, 0i64, v11);
        std::wstring::_Tidy_deallocate((__int64)v14);
      }
      else
      {
        mi::MiValue::GetValue<int>(&v16, &v13);
        mi::MiValue::GetValue<unsigned char>(&v18, (unsigned __int8 *)v12);
        if ( v12[0] == 1 )
          (*(void (__fastcall **)(_QWORD, _QWORD, _QWORD, _QWORD))this)((unsigned int)v13, 0i64, 0i64, 0i64);
      }
      v18.vtable_00 = &mi::MiValue::`vftable';
      std::wstring::_Tidy_deallocate((__int64)v18.name_08);
      v16.vtable_00 = &mi::MiValue::`vftable';
      std::wstring::_Tidy_deallocate((__int64)v16.name_08);
      v5 += 56i64;
    }
    while ( v5 != v6 );
  }
}


==========

FUNCTION: ?get_RequestGuid@WindowsFeature@ServerManager@@AEAA?AVMiInstance@mi@@XZ @ 0x180011C24
----------
__int64 __fastcall ServerManager::WindowsFeature::get_RequestGuid(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v16 = a2;
  v4 = 0i64;
  *(_QWORD *)a2 = 0i64;
  *(_QWORD *)(a2 + 8) = 0i64;
  *(_QWORD *)(a2 + 16) = 0i64;
  *(_QWORD *)(a2 + 24) = 0i64;
  *(_QWORD *)(a2 + 32) = 0i64;
  *(_QWORD *)(a2 + 40) = 0i64;
  *(_BYTE *)(a2 + 48) = 1;
  *(_OWORD *)v14 = 0i64;
  v15 = 0i64;
  v5 = 0i64;
  v6 = cxl::Guid::NewGuid((UUID *)v18);
  cxl::Guid::ToBytes((struct cxl::Guid *)v6, (__int64)v14);
  v7 = *(_QWORD *)(a1 + 32);
  std::wstring::wstring(v17, L"MSFT_ServerManagerRequestGuid");
  *(_OWORD *)v18 = 0i64;
  mi::MiApplication::CreateInstance(v7, &v19, (__int64)v17, (__int64)v18);
  if ( v18[1] )
    std::_Ref_count_base::_Decref(v18[1]);
  mi::MiInstance::operator=(a2, (__int64)&v19);
  mi::MiInstance::~MiInstance(&v19);
  std::wstring::_Tidy_deallocate((__int64)v17);
  v8 = (LODWORD(v14[1]) - LODWORD(v14[0])) /*signed*// 2 - 1;
  for ( i = v8; i /*signed*/>= 0; --i )
  {
    v10 = v4 << 8;
    if ( (unsigned __int64)i >= 7 )
      v10 = v4;
    v4 = *(unsigned __int8 *)(i + v14[0]) | (unsigned __int64)v10;
    v11 = v5 << 8;
    if ( (unsigned __int64)i >= 7 )
      v11 = v5;
    v5 = *(unsigned __int8 *)(i + v14[0] + 8) | (unsigned __int64)v11;
  }
  std::wstring::wstring(v17, L"LowHalf");
  *(_OWORD *)&v13[1] = 0i64;
  *(_OWORD *)&v13[17] = 0i64;
  *(_QWORD *)v13 = v4;
  *(_OWORD *)v19.mi_object_00 = *(_OWORD *)v13;
  *(_OWORD *)v19.padding_10_0 = *(_OWORD *)&v13[16];
  *(_QWORD *)&v19.padding_18_27[8] = 0i64;
  mi::MiInstance::AddElement((struct mi::MiInstance *)a2, (__int64)v17, (enum _MI_Type)7, (union _MI_Value *)&v19, 0x2000u);
  std::wstring::_Tidy_deallocate((__int64)v17);
  std::wstring::wstring(v17, L"HighHalf");
  *(_OWORD *)&v13[1] = 0i64;
  *(_OWORD *)&v13[17] = 0i64;
  *(_QWORD *)v13 = v5;
  *(_OWORD *)v19.mi_object_00 = *(_OWORD *)v13;
  *(_OWORD *)v19.padding_10_0 = *(_OWORD *)&v13[16];
  *(_QWORD *)&v19.padding_18_27[8] = 0i64;
  mi::MiInstance::AddElement((struct mi::MiInstance *)a2, (__int64)v17, (enum _MI_Type)7, (union _MI_Value *)&v19, 0x2000u);
  std::wstring::_Tidy_deallocate((__int64)v17);
  std::vector<unsigned char>::_Tidy((__int64)v14);
  return a2;
}


==========

FUNCTION: ?is_complete@WindowsFeature@ServerManager@@AEAA_NXZ @ 0x180011E6C
----------
bool __fastcall ServerManager::WindowsFeature::is_complete(struct ServerManager::WindowsFeature *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v6 = 0i64;
  v1 = (char *)this + 144;
  if ( this != (struct ServerManager::WindowsFeature *)-144i64 )
  {
    cxl::ScopedResOwner<cxl::NonReentrantRWLock,cxl::AcquireSharedTraits<cxl::NonReentrantRWLock>>::Release((__int64)v5);
    AcquireSRWLockShared((PSRWLOCK)v1);
    _InterlockedIncrement((volatile signed __int32 *)v1 + 3);
    v6 = v1;
  }
  v3 = *((_BYTE *)this + 160);
  cxl::ScopedResOwner<cxl::NonReentrantRWLock,cxl::AcquireSharedTraits<cxl::NonReentrantRWLock>>::Release((__int64)v5);
  return v3;
}


==========

