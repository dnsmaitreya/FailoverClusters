Source file: Microsoft.FailoverClusters.FrameworkSupport
Generated time: 20260130_193001
==========

FUNCTION: ?AddFiletimeProperty@@YAKPEAU_HCLUSPROPLIST@@PEB_WPEAU_FILETIME@@@Z @ 0x18000CAD0
----------
unsigned int __fastcall AddFiletimeProperty(struct _HCLUSPROPLIST *a1, const wchar_t *a2, struct _FILETIME *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a3 )
    return 87;
  v4[0] = (__int64)a2;
  v4[1] = (__int64)a3;
  return ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_2425f16268ee7fc8619b5680899d7c35___(a1, (__int64)v4);
}


==========

FUNCTION: AddBinaryProperty @ 0x18000E0E0
----------
__int64 __fastcall AddBinaryProperty(_DWORD *a1, __int64 a2, __int64 a3, int a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a3 || !a4 )
    return 87i64;
  v5[0] = a2;
  v5[1] = a3;
  v6 = a4;
  return ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_3fb63138c47b36dfdc63320a3cb5a182___(a1, (__int64)v5);
}


==========

FUNCTION: AddDwordProperty @ 0x18000E120
----------
__int64 __fastcall AddDwordProperty(_DWORD *a1, __int64 a2, int a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = a2;
  v5 = a3;
  return ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_7f9ac22dde1fdb4133288f548b6e5d76___(a1, (__int64)&v4);
}


==========

FUNCTION: AddExpandSzProperty @ 0x18000E150
----------
__int64 __fastcall AddExpandSzProperty(_DWORD *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4[0] = a2;
  v4[1] = a3;
  return ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_8fa3ca58b8b0714c8ed3ff8d617f7830___(a1, (__int64)v4);
}


==========

FUNCTION: AddLong64Property @ 0x18000E180
----------
__int64 __fastcall AddLong64Property(_DWORD *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4[0] = a2;
  v4[1] = a3;
  return ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_d22b008dfb5300cf81d8c09b9b798147___(a1, v4);
}


==========

FUNCTION: AddLongProperty @ 0x18000E1B0
----------
__int64 __fastcall AddLongProperty(_DWORD *a1, __int64 a2, int a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = a2;
  v5 = a3;
  return ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_b30ea29c6b30300efa40cc116c4a4557___(a1, &v4);
}


==========

FUNCTION: AddMultiSzProperty @ 0x18000E1E0
----------
__int64 __fastcall AddMultiSzProperty(_DWORD *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4[0] = a2;
  v4[1] = a3;
  return ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_df0e274ade00e93eccae715afab64c6d___(a1, (__int64)v4);
}


==========

FUNCTION: AddStringProperty @ 0x18000E210
----------
__int64 __fastcall AddStringProperty(_DWORD *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4[0] = a2;
  v4[1] = a3;
  return ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_cb47984686c7bc8f1bd86ee836172e3e___(a1, (__int64)v4);
}


==========

FUNCTION: AddULong64Property @ 0x18000E240
----------
__int64 __fastcall AddULong64Property(_DWORD *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4[0] = a2;
  v4[1] = a3;
  return ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_b40cc3a0d7d9f0009e697a019c46cbdc___(a1, v4);
}


==========

FUNCTION: AddWordProperty @ 0x18000E270
----------
__int64 __fastcall AddWordProperty(_DWORD *a1, __int64 a2, __int16 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = a2;
  v5 = a3;
  return ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_50312152ad7f113be312b0ed9d662ae6___(a1, &v4);
}


==========

Source file: Microsoft.FailoverClusters.FrameworkSupport
Generated time: 20260130_193041
==========

FUNCTION: ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_07a3c0bbec807ad4adce81e940cbcb18___ @ 0x180008F80
----------
__int64 __fastcall ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_07a3c0bbec807ad4adce81e940cbcb18___(_DWORD *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  cxl::PropertyList::property_iterator::property_iterator((struct cxl::PropertyList::property_iterator *)v14);
  *(_QWORD *)&v9 = a1;
  BYTE8(v9) = 1;
  v10 = (__int64)&std::_Func_impl_no_alloc<_lambda_a9f9942fab941612934a47099c877bdf_,void,>::`vftable';
  v11 = v9;
  v12 = &v10;
  cxl::LambdaScopeGuard::LambdaScopeGuard(v13, (__int64)&v10);
  if ( v12 )
  {
    v5 = &v10;
    LOBYTE(v5) = v12 != &v10;
    (*(void (__fastcall **)(__int64 *, __int64 *))(*v12 + 32))(v12, v5);
    v12 = 0i64;
  }
  PropList::ListHandle<cxl::PropertyList>::operator++((__int64)a1, (__int64)v15);
  PropList::ListHandle<cxl::PropertyList>::~ListHandle<cxl::PropertyList>((__int64)v15);
  cxl::PropertyList::property_iterator::operator=((__int64)v14, (__int64)(a1 + 12));
  v6 = PropList::ListHandle<cxl::PropertyList>::end((__int64)a1, (__int64)v15);
  v7 = cxl::PropertyList::property_iterator::operator==(v6, (__int64)v14);
  cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v15);
  if ( v7 )
  {
    cxl::LambdaScopeGuard::~LambdaScopeGuard(v13);
    cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v14);
    result = 259i64;
  }
  else
  {
    lambda_07a3c0bbec807ad4adce81e940cbcb18_::operator()(a3, v14);
    v13[0].active_00 = 0;
    cxl::LambdaScopeGuard::~LambdaScopeGuard(v13);
    cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v14);
    result = 0i64;
  }
  return result;
}


==========

FUNCTION: ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_2566e9e25065fa32b9a9bdfc344e183e___ @ 0x180009124
----------
__int64 __fastcall ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_2566e9e25065fa32b9a9bdfc344e183e___(_DWORD *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  cxl::PropertyList::property_iterator::property_iterator((struct cxl::PropertyList::property_iterator *)v14);
  *(_QWORD *)&v9 = a1;
  BYTE8(v9) = 0;
  v10 = (__int64)&std::_Func_impl_no_alloc<_lambda_570229cf05cb21bd60f6cce7728c7501_,void,>::`vftable';
  v11 = v9;
  v12 = &v10;
  cxl::LambdaScopeGuard::LambdaScopeGuard(v13, (__int64)&v10);
  if ( v12 )
  {
    v5 = &v10;
    LOBYTE(v5) = v12 != &v10;
    (*(void (__fastcall **)(__int64 *, __int64 *))(*v12 + 32))(v12, v5);
    v12 = 0i64;
  }
  cxl::PropertyList::property_iterator::operator=((__int64)v14, (__int64)(a1 + 12));
  v6 = PropList::ListHandle<cxl::PropertyList>::end((__int64)a1, (__int64)v15);
  v7 = cxl::PropertyList::property_iterator::operator==(v6, (__int64)v14);
  cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v15);
  if ( v7 )
  {
    cxl::LambdaScopeGuard::~LambdaScopeGuard(v13);
    cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v14);
    result = 259i64;
  }
  else
  {
    lambda_2566e9e25065fa32b9a9bdfc344e183e_::operator()((_DWORD **)a3, v14);
    v13[0].active_00 = 0;
    cxl::LambdaScopeGuard::~LambdaScopeGuard(v13);
    cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v14);
    result = 0i64;
  }
  return result;
}


==========

FUNCTION: ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_6bd7d65065ccc55fa2a4354bff4a4e86___ @ 0x1800092AC
----------
__int64 __fastcall ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_6bd7d65065ccc55fa2a4354bff4a4e86___(_DWORD *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  cxl::PropertyList::property_iterator::property_iterator((struct cxl::PropertyList::property_iterator *)v14);
  *(_QWORD *)&v9 = a1;
  BYTE8(v9) = 0;
  v10 = (__int64)&std::_Func_impl_no_alloc<_lambda_d0a9deb2c895958a168d6e9c12164e37_,void,>::`vftable';
  v11 = v9;
  v12 = &v10;
  cxl::LambdaScopeGuard::LambdaScopeGuard(v13, (__int64)&v10);
  if ( v12 )
  {
    v5 = &v10;
    LOBYTE(v5) = v12 != &v10;
    (*(void (__fastcall **)(__int64 *, __int64 *))(*v12 + 32))(v12, v5);
    v12 = 0i64;
  }
  cxl::PropertyList::property_iterator::operator=((__int64)v14, (__int64)(a1 + 12));
  v6 = PropList::ListHandle<cxl::PropertyList>::end((__int64)a1, (__int64)v15);
  v7 = cxl::PropertyList::property_iterator::operator==(v6, (__int64)v14);
  cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v15);
  if ( v7 )
  {
    cxl::LambdaScopeGuard::~LambdaScopeGuard(v13);
    cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v14);
    result = 259i64;
  }
  else
  {
    lambda_6bd7d65065ccc55fa2a4354bff4a4e86_::operator()((_DWORD **)a3, v14);
    v13[0].active_00 = 0;
    cxl::LambdaScopeGuard::~LambdaScopeGuard(v13);
    cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v14);
    result = 0i64;
  }
  return result;
}


==========

FUNCTION: ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_77dd2ded7b3adeddb7d91c00f3afaa57___ @ 0x180009434
----------
__int64 __fastcall ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_77dd2ded7b3adeddb7d91c00f3afaa57___(_DWORD *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  cxl::PropertyList::property_iterator::property_iterator((struct cxl::PropertyList::property_iterator *)v14);
  *(_QWORD *)&v9 = a1;
  BYTE8(v9) = 0;
  v10 = (__int64)&std::_Func_impl_no_alloc<_lambda_76134e02efb98e16824f81281b06d89c_,void,>::`vftable';
  v11 = v9;
  v12 = &v10;
  cxl::LambdaScopeGuard::LambdaScopeGuard(v13, (__int64)&v10);
  if ( v12 )
  {
    v5 = &v10;
    LOBYTE(v5) = v12 != &v10;
    (*(void (__fastcall **)(__int64 *, __int64 *))(*v12 + 32))(v12, v5);
    v12 = 0i64;
  }
  cxl::PropertyList::property_iterator::operator=((__int64)v14, (__int64)(a1 + 12));
  v6 = PropList::ListHandle<cxl::PropertyList>::end((__int64)a1, (__int64)v15);
  v7 = cxl::PropertyList::property_iterator::operator==(v6, (__int64)v14);
  cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v15);
  if ( v7 )
  {
    cxl::LambdaScopeGuard::~LambdaScopeGuard(v13);
    cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v14);
    result = 259i64;
  }
  else
  {
    lambda_77dd2ded7b3adeddb7d91c00f3afaa57_::operator()((__int64 **)a3, v14);
    v13[0].active_00 = 0;
    cxl::LambdaScopeGuard::~LambdaScopeGuard(v13);
    cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v14);
    result = 0i64;
  }
  return result;
}


==========

FUNCTION: ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_b7fc88a87856aae459d6ca2a17683ada___ @ 0x1800095BC
----------
__int64 __fastcall ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_b7fc88a87856aae459d6ca2a17683ada___(_DWORD *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  cxl::PropertyList::property_iterator::property_iterator((struct cxl::PropertyList::property_iterator *)v14);
  *(_QWORD *)&v9 = a1;
  BYTE8(v9) = 0;
  v10 = (__int64)&std::_Func_impl_no_alloc<_lambda_304633c323c1ff9f0c64ffea633b2634_,void,>::`vftable';
  v11 = v9;
  v12 = &v10;
  cxl::LambdaScopeGuard::LambdaScopeGuard(v13, (__int64)&v10);
  if ( v12 )
  {
    v5 = &v10;
    LOBYTE(v5) = v12 != &v10;
    (*(void (__fastcall **)(__int64 *, __int64 *))(*v12 + 32))(v12, v5);
    v12 = 0i64;
  }
  cxl::PropertyList::property_iterator::operator=((__int64)v14, (__int64)(a1 + 12));
  v6 = PropList::ListHandle<cxl::PropertyList>::end((__int64)a1, (__int64)v15);
  v7 = cxl::PropertyList::property_iterator::operator==(v6, (__int64)v14);
  cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v15);
  if ( v7 )
  {
    cxl::LambdaScopeGuard::~LambdaScopeGuard(v13);
    cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v14);
    result = 259i64;
  }
  else
  {
    lambda_b7fc88a87856aae459d6ca2a17683ada_::operator()(a3, v14);
    v13[0].active_00 = 0;
    cxl::LambdaScopeGuard::~LambdaScopeGuard(v13);
    cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v14);
    result = 0i64;
  }
  return result;
}


==========

FUNCTION: ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_bc219e522c0b4f853fcbee724f644237___ @ 0x180009744
----------
__int64 __fastcall ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_bc219e522c0b4f853fcbee724f644237___(_DWORD *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  cxl::PropertyList::property_iterator::property_iterator((struct cxl::PropertyList::property_iterator *)v14);
  *(_QWORD *)&v9 = a1;
  BYTE8(v9) = 0;
  v10 = (__int64)&std::_Func_impl_no_alloc<_lambda_f029ec8a2b98e44d9beb2d33d1b92050_,void,>::`vftable';
  v11 = v9;
  v12 = &v10;
  cxl::LambdaScopeGuard::LambdaScopeGuard(v13, (__int64)&v10);
  if ( v12 )
  {
    v5 = &v10;
    LOBYTE(v5) = v12 != &v10;
    (*(void (__fastcall **)(__int64 *, __int64 *))(*v12 + 32))(v12, v5);
    v12 = 0i64;
  }
  cxl::PropertyList::property_iterator::operator=((__int64)v14, (__int64)(a1 + 12));
  v6 = PropList::ListHandle<cxl::PropertyList>::end((__int64)a1, (__int64)v15);
  v7 = cxl::PropertyList::property_iterator::operator==(v6, (__int64)v14);
  cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v15);
  if ( v7 )
  {
    cxl::LambdaScopeGuard::~LambdaScopeGuard(v13);
    cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v14);
    result = 259i64;
  }
  else
  {
    lambda_bc219e522c0b4f853fcbee724f644237_::operator()((__int64 **)a3, v14);
    v13[0].active_00 = 0;
    cxl::LambdaScopeGuard::~LambdaScopeGuard(v13);
    cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v14);
    result = 0i64;
  }
  return result;
}


==========

FUNCTION: ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_d05859d272d8f20f18d1d63fb4a96038___ @ 0x1800098CC
----------
__int64 __fastcall ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_d05859d272d8f20f18d1d63fb4a96038___(_DWORD *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  cxl::PropertyList::property_iterator::property_iterator((struct cxl::PropertyList::property_iterator *)v14);
  *(_QWORD *)&v9 = a1;
  BYTE8(v9) = 0;
  v10 = (__int64)&std::_Func_impl_no_alloc<_lambda_61c6f345fcf34b7e9c1eca5836e848ff_,void,>::`vftable';
  v11 = v9;
  v12 = &v10;
  cxl::LambdaScopeGuard::LambdaScopeGuard(v13, (__int64)&v10);
  if ( v12 )
  {
    v5 = &v10;
    LOBYTE(v5) = v12 != &v10;
    (*(void (__fastcall **)(__int64 *, __int64 *))(*v12 + 32))(v12, v5);
    v12 = 0i64;
  }
  cxl::PropertyList::property_iterator::operator=((__int64)v14, (__int64)(a1 + 12));
  v6 = PropList::ListHandle<cxl::PropertyList>::end((__int64)a1, (__int64)v15);
  v7 = cxl::PropertyList::property_iterator::operator==(v6, (__int64)v14);
  cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v15);
  if ( v7 )
  {
    cxl::LambdaScopeGuard::~LambdaScopeGuard(v13);
    cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v14);
    result = 259i64;
  }
  else
  {
    lambda_d05859d272d8f20f18d1d63fb4a96038_::operator()(a3, v14);
    v13[0].active_00 = 0;
    cxl::LambdaScopeGuard::~LambdaScopeGuard(v13);
    cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v14);
    result = 0i64;
  }
  return result;
}


==========

FUNCTION: ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_f381ae270b5b223c6e9389e4670f425a___ @ 0x180009A54
----------
__int64 __fastcall ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_f381ae270b5b223c6e9389e4670f425a___(_DWORD *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  cxl::PropertyList::property_iterator::property_iterator((struct cxl::PropertyList::property_iterator *)v14);
  *(_QWORD *)&v9 = a1;
  BYTE8(v9) = 0;
  v10 = (__int64)&std::_Func_impl_no_alloc<_lambda_ff1a1b9afaa53fada1cbfb02eca14e3b_,void,>::`vftable';
  v11 = v9;
  v12 = &v10;
  cxl::LambdaScopeGuard::LambdaScopeGuard(v13, (__int64)&v10);
  if ( v12 )
  {
    v5 = &v10;
    LOBYTE(v5) = v12 != &v10;
    (*(void (__fastcall **)(__int64 *, __int64 *))(*v12 + 32))(v12, v5);
    v12 = 0i64;
  }
  cxl::PropertyList::property_iterator::operator=((__int64)v14, (__int64)(a1 + 12));
  v6 = PropList::ListHandle<cxl::PropertyList>::end((__int64)a1, (__int64)v15);
  v7 = cxl::PropertyList::property_iterator::operator==(v6, (__int64)v14);
  cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v15);
  if ( v7 )
  {
    cxl::LambdaScopeGuard::~LambdaScopeGuard(v13);
    cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v14);
    result = 259i64;
  }
  else
  {
    lambda_f381ae270b5b223c6e9389e4670f425a_::operator()(a3, v14);
    v13[0].active_00 = 0;
    cxl::LambdaScopeGuard::~LambdaScopeGuard(v13);
    cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v14);
    result = 0i64;
  }
  return result;
}


==========

FUNCTION: ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_f66efe11f75844ca792b3ca9eb2a16e8___ @ 0x180009BDC
----------
__int64 __fastcall ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_f66efe11f75844ca792b3ca9eb2a16e8___(_DWORD *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  cxl::PropertyList::property_iterator::property_iterator((struct cxl::PropertyList::property_iterator *)v14);
  *(_QWORD *)&v9 = a1;
  BYTE8(v9) = 0;
  v10 = (__int64)&std::_Func_impl_no_alloc<_lambda_c056010fe419f82bc3457edda267ca76_,void,>::`vftable';
  v11 = v9;
  v12 = &v10;
  cxl::LambdaScopeGuard::LambdaScopeGuard(v13, (__int64)&v10);
  if ( v12 )
  {
    v5 = &v10;
    LOBYTE(v5) = v12 != &v10;
    (*(void (__fastcall **)(__int64 *, __int64 *))(*v12 + 32))(v12, v5);
    v12 = 0i64;
  }
  cxl::PropertyList::property_iterator::operator=((__int64)v14, (__int64)(a1 + 12));
  v6 = PropList::ListHandle<cxl::PropertyList>::end((__int64)a1, (__int64)v15);
  v7 = cxl::PropertyList::property_iterator::operator==(v6, (__int64)v14);
  cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v15);
  if ( v7 )
  {
    cxl::LambdaScopeGuard::~LambdaScopeGuard(v13);
    cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v14);
    result = 259i64;
  }
  else
  {
    lambda_f66efe11f75844ca792b3ca9eb2a16e8_::operator()((__int64 **)a3, v14);
    v13[0].active_00 = 0;
    cxl::LambdaScopeGuard::~LambdaScopeGuard(v13);
    cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v14);
    result = 0i64;
  }
  return result;
}


==========

FUNCTION: ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_f86cfdd64835514781773bc5c43f1464___ @ 0x180009D64
----------
__int64 __fastcall ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_f86cfdd64835514781773bc5c43f1464___(_DWORD *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  cxl::PropertyList::property_iterator::property_iterator((struct cxl::PropertyList::property_iterator *)v14);
  *(_QWORD *)&v9 = a1;
  BYTE8(v9) = 0;
  v10 = (__int64)&std::_Func_impl_no_alloc<_lambda_692c299d4aa3820a51a15e2d93df1634_,void,>::`vftable';
  v11 = v9;
  v12 = &v10;
  cxl::LambdaScopeGuard::LambdaScopeGuard(v13, (__int64)&v10);
  if ( v12 )
  {
    v5 = &v10;
    LOBYTE(v5) = v12 != &v10;
    (*(void (__fastcall **)(__int64 *, __int64 *))(*v12 + 32))(v12, v5);
    v12 = 0i64;
  }
  cxl::PropertyList::property_iterator::operator=((__int64)v14, (__int64)(a1 + 12));
  v6 = PropList::ListHandle<cxl::PropertyList>::end((__int64)a1, (__int64)v15);
  v7 = cxl::PropertyList::property_iterator::operator==(v6, (__int64)v14);
  cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v15);
  if ( v7 )
  {
    cxl::LambdaScopeGuard::~LambdaScopeGuard(v13);
    cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v14);
    result = 259i64;
  }
  else
  {
    lambda_f86cfdd64835514781773bc5c43f1464_::operator()((_WORD **)a3, v14);
    v13[0].active_00 = 0;
    cxl::LambdaScopeGuard::~LambdaScopeGuard(v13);
    cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v14);
    result = 0i64;
  }
  return result;
}


==========

FUNCTION: ExecuteOnIterator_PropList::ListHandle_cxl::ValueList___lambda_89b374e4cadd23178fd818a361f551cf___ @ 0x180009EEC
----------
__int64 __fastcall ExecuteOnIterator_PropList::ListHandle_cxl::ValueList___lambda_89b374e4cadd23178fd818a361f551cf___(__int64 a1, unsigned __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *(_DWORD *)a1 != 1 )
    return 6i64;
  cxl::ValueList::value_iterator::value_iterator((struct cxl::ValueList::value_iterator *)&v9, a2);
  v8.buffer_ptr_00 = (void *)a1;
  LOBYTE(v8.list_ptr_08) = 1;
  v16 = (__int64)&std::_Func_impl_no_alloc<_lambda_5e8b8c505bc6c59639978a7a85723a34_,void,>::`vftable';
  v17 = *(_OWORD *)&v8.buffer_ptr_00;
  v18 = &v16;
  cxl::LambdaScopeGuard::LambdaScopeGuard(v19, (__int64)&v16);
  if ( v18 )
  {
    v5 = &v16;
    LOBYTE(v5) = v18 != &v16;
    (*(void (__fastcall **)(__int64 *, __int64 *))(*v18 + 32))(v18, v5);
    v18 = 0i64;
  }
  PropList::ListHandle<cxl::ValueList>::ListHandle<cxl::ValueList>((__int64)v20, a1);
  PropList::ListHandle<cxl::ValueList>::next(a1);
  PropList::ListHandle<cxl::ValueList>::~ListHandle<cxl::ValueList>(v20);
  v9 = *(_QWORD *)(a1 + 96);
  std::shared_ptr<mi::MiApplication>::operator=((__int64 *)v10);
  v12 = *(_QWORD *)(a1 + 120);
  v13 = *(_QWORD *)(a1 + 128);
  *(_OWORD *)&v8.buffer_ptr_00 = (unsigned __int64)(a1 + 8);
  v8.flag_10 = 0;
  cxl::ValueList::value_iterator::value_iterator((struct cxl::ValueList::value_iterator *)&v14, 0xFFFFFFFFFFFFFFFFui64, &v8);
  v6 = v14 == v9;
  if ( v15 )
    std::_Ref_count_base::_Decref(v15);
  if ( v6 )
  {
    cxl::LambdaScopeGuard::~LambdaScopeGuard(v19);
    if ( v11 )
      std::_Ref_count_base::_Decref(v11);
    result = 259i64;
  }
  else
  {
    lambda_89b374e4cadd23178fd818a361f551cf_::operator()(a3, (__int64)&v9);
    v19[0].active_00 = 0;
    cxl::LambdaScopeGuard::~LambdaScopeGuard(v19);
    if ( v11 )
      std::_Ref_count_base::_Decref(v11);
    result = 0i64;
  }
  return result;
}


==========

FUNCTION: ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_17de830c8f2ff13b33ff664c5e949495___ @ 0x18000A0EC
----------
__int64 __fastcall ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_17de830c8f2ff13b33ff664c5e949495___(_DWORD *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  lambda_17de830c8f2ff13b33ff664c5e949495_::operator()((_WORD **)a2, (cxl::PropertyList *)(a1 + 2));
  return 0i64;
}


==========

FUNCTION: ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_1d662bbbc3dfdbd0b967971505b0be17___ @ 0x18000A12C
----------
__int64 __fastcall ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_1d662bbbc3dfdbd0b967971505b0be17___(_DWORD *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  **(_DWORD **)(a2 + 8) = a1[8] - a1[6];
  **(_QWORD **)a2 = *((_QWORD *)a1 + 3);
  return 0i64;
}


==========

FUNCTION: ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_2425f16268ee7fc8619b5680899d7c35___ @ 0x18000A178
----------
__int64 __fastcall ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_2425f16268ee7fc8619b5680899d7c35___(_DWORD *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  v3 = **(_QWORD **)(a2 + 8);
  std::wstring::wstring(v8, *(_WORD **)a2);
  v6 = v3;
  v5 = 65548;
  v7[0] = (__int64)&v6;
  v7[1] = (__int64)&v5;
  cxl::PropertyList::AddPropertyT<_lambda_866afcf6517d38a17cd38b5b648355d1_>((struct cxl::PropertyList *)(a1 + 2), (__int64)v8, (const struct _lambda_866afcf6517d38a17cd38b5b648355d1_ *)v7);
  std::wstring::_Tidy_deallocate((__int64)v8);
  return 0i64;
}


==========

FUNCTION: ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_3fb63138c47b36dfdc63320a3cb5a182___ @ 0x18000A230
----------
__int64 __fastcall ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_3fb63138c47b36dfdc63320a3cb5a182___(_DWORD *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  lambda_3fb63138c47b36dfdc63320a3cb5a182_::operator()(a2, (__int64)(a1 + 2));
  return 0i64;
}


==========

FUNCTION: ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_50312152ad7f113be312b0ed9d662ae6___ @ 0x18000A270
----------
__int64 __fastcall ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_50312152ad7f113be312b0ed9d662ae6___(_DWORD *a1, _QWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  std::wstring::wstring(v7, (_WORD *)*a2);
  v5 = 65547;
  v6[0] = (__int64)(a2 + 1);
  v6[1] = (__int64)&v5;
  cxl::PropertyList::AddPropertyT<_lambda_923022901fc03a9890276e9013447403_>((struct cxl::PropertyList *)(a1 + 2), (__int64)v7, (const struct _lambda_923022901fc03a9890276e9013447403_ *)v6);
  std::wstring::_Tidy_deallocate((__int64)v7);
  return 0i64;
}


==========

FUNCTION: ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_7f9ac22dde1fdb4133288f548b6e5d76___ @ 0x18000A320
----------
__int64 __fastcall ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_7f9ac22dde1fdb4133288f548b6e5d76___(_DWORD *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  v3 = *(_DWORD *)(a2 + 8);
  std::wstring::wstring(v8, *(_WORD **)a2);
  v6 = v3;
  v5 = 65538;
  v7[0] = (__int64)&v6;
  v7[1] = (__int64)&v5;
  cxl::PropertyList::AddPropertyT<_lambda_a945c21e4f5b4a83c77c0f81b6ab26fd_>((struct cxl::PropertyList *)(a1 + 2), (__int64)v8, (const struct _lambda_a945c21e4f5b4a83c77c0f81b6ab26fd_ *)v7);
  std::wstring::_Tidy_deallocate((__int64)v8);
  return 0i64;
}


==========

FUNCTION: ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_8fa3ca58b8b0714c8ed3ff8d617f7830___ @ 0x18000A3D4
----------
__int64 __fastcall ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_8fa3ca58b8b0714c8ed3ff8d617f7830___(_DWORD *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  lambda_8fa3ca58b8b0714c8ed3ff8d617f7830_::operator()((_WORD **)a2, (__int64)(a1 + 2));
  return 0i64;
}


==========

FUNCTION: ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_b30ea29c6b30300efa40cc116c4a4557___ @ 0x18000A414
----------
__int64 __fastcall ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_b30ea29c6b30300efa40cc116c4a4557___(_DWORD *a1, _QWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  std::wstring::wstring(v7, (_WORD *)*a2);
  v5 = 65543;
  v6[0] = (__int64)(a2 + 1);
  v6[1] = (__int64)&v5;
  cxl::PropertyList::AddPropertyT<_lambda_dc7f9f5f34eab37b60d6e390d88b50ff_>((struct cxl::PropertyList *)(a1 + 2), (__int64)v7, (const struct _lambda_dc7f9f5f34eab37b60d6e390d88b50ff_ *)v6);
  std::wstring::_Tidy_deallocate((__int64)v7);
  return 0i64;
}


==========

FUNCTION: ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_b40cc3a0d7d9f0009e697a019c46cbdc___ @ 0x18000A4C4
----------
__int64 __fastcall ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_b40cc3a0d7d9f0009e697a019c46cbdc___(_DWORD *a1, _QWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  v3 = a2[1];
  std::wstring::wstring(v8, (_WORD *)*a2);
  v6 = v3;
  v5 = 65542;
  v7[0] = (__int64)&v6;
  v7[1] = (__int64)&v5;
  cxl::PropertyList::AddPropertyT<_lambda_70c4d3f05d475c5512ec25d928cee14c_>((struct cxl::PropertyList *)(a1 + 2), (__int64)v8, (const struct _lambda_70c4d3f05d475c5512ec25d928cee14c_ *)v7);
  std::wstring::_Tidy_deallocate((__int64)v8);
  return 0i64;
}


==========

FUNCTION: ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_b573ff8a43627b5b8bf6c7be37b87c4d___ @ 0x18000A578
----------
__int64 __fastcall ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_b573ff8a43627b5b8bf6c7be37b87c4d___(_DWORD *a1, void *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  v3 = cxl::PropertyList::PropertyList(&v5, *(struct CLUSPROP_LIST *const *)a2, *((unsigned int *)a2 + 2));
  cxl::PropertyList::operator=((__int64)(a1 + 2), v3);
  cxl::PropertyList::~PropertyList(&v5);
  return 0i64;
}


==========

FUNCTION: ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_cb47984686c7bc8f1bd86ee836172e3e___ @ 0x18000A5DC
----------
__int64 __fastcall ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_cb47984686c7bc8f1bd86ee836172e3e___(_DWORD *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  lambda_cb47984686c7bc8f1bd86ee836172e3e_::operator()((_WORD **)a2, (__int64)(a1 + 2));
  return 0i64;
}


==========

FUNCTION: ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_d22b008dfb5300cf81d8c09b9b798147___ @ 0x18000A61C
----------
__int64 __fastcall ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_d22b008dfb5300cf81d8c09b9b798147___(_DWORD *a1, _QWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  v3 = a2[1];
  std::wstring::wstring(v8, (_WORD *)*a2);
  v6 = v3;
  v5 = 65546;
  v7[0] = (__int64)&v6;
  v7[1] = (__int64)&v5;
  cxl::PropertyList::AddPropertyT<_lambda_07df422df13fe0082e8caa41363132ed_>((struct cxl::PropertyList *)(a1 + 2), (__int64)v8, (const struct _lambda_07df422df13fe0082e8caa41363132ed_ *)v7);
  std::wstring::_Tidy_deallocate((__int64)v8);
  return 0i64;
}


==========

FUNCTION: ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_df0e274ade00e93eccae715afab64c6d___ @ 0x18000A6D0
----------
__int64 __fastcall ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_df0e274ade00e93eccae715afab64c6d___(_DWORD *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  lambda_df0e274ade00e93eccae715afab64c6d_::operator()((_WORD **)a2, (__int64)(a1 + 2));
  return 0i64;
}


==========

FUNCTION: ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_ee6f54c70da574f83b443140c6fe1cc5___ @ 0x18000A710
----------
__int64 __fastcall ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_ee6f54c70da574f83b443140c6fe1cc5___(__int64 a1, _DWORD **a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *(_DWORD *)a1 != 1 )
    return 6i64;
  **a2 = **(_DWORD **)(a1 + 24);
  return 0i64;
}


==========

FUNCTION: ExecuteOnList_PropList::ListHandle_cxl::ValueList___lambda_d968e976aea494ce79ca4695ff02f82d___ @ 0x18000A74C
----------
__int64 __fastcall ExecuteOnList_PropList::ListHandle_cxl::ValueList___lambda_d968e976aea494ce79ca4695ff02f82d___(_DWORD *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a1 || *a1 != 1 )
    return 6i64;
  v3 = cxl::ValueList::ValueList(&v5, *(const unsigned __int8 *const *)a2, *(unsigned int *)(a2 + 8));
  cxl::ValueList::operator=((_QWORD *)a1 + 1, v3);
  cxl::ValueList::~ValueList(&v5);
  return 0i64;
}


==========

Source file: Microsoft.FailoverClusters.FrameworkSupport
Generated time: 20260130_193505
==========

FUNCTION: GetFiletimeProperty @ 0x18000E530
----------
__int64 __fastcall GetFiletimeProperty(_DWORD *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = a2;
  return ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_77dd2ded7b3adeddb7d91c00f3afaa57___(a1, a2, (__int64)&v3);
}


==========

FUNCTION: GetLong64Property @ 0x18000E580
----------
__int64 __fastcall GetLong64Property(_DWORD *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = a2;
  return ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_bc219e522c0b4f853fcbee724f644237___(a1, a2, (__int64)&v3);
}


==========

FUNCTION: GetLongProperty @ 0x18000E5A0
----------
__int64 __fastcall GetLongProperty(_DWORD *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = a2;
  return ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_2566e9e25065fa32b9a9bdfc344e183e___(a1, a2, (__int64)&v3);
}


==========

FUNCTION: GetMultiSzProperty @ 0x18000E5C0
----------
__int64 __fastcall GetMultiSzProperty(_DWORD *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4[1] = a3;
  v4[0] = a2;
  return ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_f381ae270b5b223c6e9389e4670f425a___(a1, a2, (__int64)v4);
}


==========

FUNCTION: GetNextProperty @ 0x18000E5F0
----------
__int64 __fastcall GetNextProperty(_DWORD *a1, __int64 a2, __int64 a3, __int64 a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a3 )
  {
    if ( a4 )
    {
LABEL_3:
      v5[0] = a3;
      v5[1] = a4;
      v5[2] = a2;
      return ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_07a3c0bbec807ad4adce81e940cbcb18___(a1, a2, (__int64)v5);
    }
  }
  else if ( !a4 )
  {
    goto LABEL_3;
  }
  return 87i64;
}


==========

FUNCTION: GetNextValue @ 0x18000E630
----------
__int64 __fastcall GetNextValue(__int64 a1, unsigned __int64 a2, __int64 a3, __int64 a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a3 )
  {
    if ( a4 )
    {
LABEL_3:
      v5[1] = a3;
      v5[0] = a2;
      v5[2] = a4;
      return ExecuteOnIterator_PropList::ListHandle_cxl::ValueList___lambda_89b374e4cadd23178fd818a361f551cf___(a1, a2, (__int64)v5);
    }
  }
  else if ( !a4 )
  {
    goto LABEL_3;
  }
  return 87i64;
}


==========

FUNCTION: GetPropertyCount @ 0x18000E670
----------
__int64 __fastcall GetPropertyCount(__int64 a1, _DWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = a2;
  return ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_ee6f54c70da574f83b443140c6fe1cc5___(a1, &v3);
}


==========

FUNCTION: GetPropertyListBuffer @ 0x18000E690
----------
__int64 __fastcall GetPropertyListBuffer(_DWORD *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4[0] = a2;
  v4[1] = a3;
  return ExecuteOnList_PropList::ListHandle_cxl::PropertyList___lambda_1d662bbbc3dfdbd0b967971505b0be17___(a1, (__int64)v4);
}


==========

FUNCTION: GetStringProperty @ 0x18000E6C0
----------
__int64 __fastcall GetStringProperty(_DWORD *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a2 )
  {
    if ( a3 )
      goto LABEL_3;
  }
  else if ( !a3 )
  {
LABEL_3:
    v4[1] = a3;
    v4[0] = a2;
    return ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_b7fc88a87856aae459d6ca2a17683ada___(a1, a2, (__int64)v4);
  }
  return 87i64;
}


==========

FUNCTION: GetULong64Property @ 0x18000E700
----------
__int64 __fastcall GetULong64Property(_DWORD *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = a2;
  return ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_f66efe11f75844ca792b3ca9eb2a16e8___(a1, a2, (__int64)&v3);
}


==========

FUNCTION: GetWordProperty @ 0x18000E720
----------
__int64 __fastcall GetWordProperty(_DWORD *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = a2;
  return ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_f86cfdd64835514781773bc5c43f1464___(a1, a2, (__int64)&v3);
}


==========

Source file: Microsoft.FailoverClusters.FrameworkSupport
Generated time: 20260130_193716
==========

FUNCTION: GetBinaryProperty @ 0x18000E4D0
----------
__int64 __fastcall GetBinaryProperty(_DWORD *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4[1] = a3;
  v4[0] = a2;
  return ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_d05859d272d8f20f18d1d63fb4a96038___(a1, a2, (__int64)v4);
}


==========

