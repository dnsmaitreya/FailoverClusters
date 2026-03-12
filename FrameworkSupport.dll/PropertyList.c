Source file: Microsoft.FailoverClusters.FrameworkSupport
Generated time: 20260130_192601
==========

FUNCTION: ??1PropertyData@cxl@@QEAA@XZ @ 0x18000B8D4
----------
void __fastcall cxl::PropertyData::~PropertyData(struct cxl::PropertyData *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::wstring::_Tidy_deallocate((__int64)this + 24);
  v2 = (std::_Ref_count_base *)*((_QWORD *)this + 1);
  if ( v2 )
    std::_Ref_count_base::_Decref(v2);
}


==========

FUNCTION: ??1property_iterator@PropertyList@cxl@@QEAA@XZ @ 0x18000B918
----------
void __fastcall cxl::PropertyList::property_iterator::~property_iterator(struct cxl::PropertyList::property_iterator *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = (std::_Ref_count_base *)*((_QWORD *)this + 10);
  if ( v2 )
    std::_Ref_count_base::_Decref(v2);
  cxl::PropertyData::~PropertyData(this);
}


==========

FUNCTION: ??$AddPropertyT@V_lambda_07df422df13fe0082e8caa41363132ed_@@@PropertyList@cxl@@AEAA_NAEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEBV_lambda_07df422df13fe0082e8caa41363132ed_@@@Z @ 0x1800126B4
----------
bool __fastcall cxl::PropertyList::AddPropertyT<_lambda_07df422df13fe0082e8caa41363132ed_>(struct cxl::PropertyList *this, __int64 a2, const struct _lambda_07df422df13fe0082e8caa41363132ed_ *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v17 = 0i64;
  std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::find((__int64 *)this, &v14, a2);
  if ( (struct CLUSPROP_LIST *)v14 != this->CLUSPROP_LIST )
  {
    v17 = *(_OWORD *)(v14 + 64);
    v6 = this->field_10;
    for ( i = (_DWORD *)(v6 + *(_QWORD *)(v14 + 72)); *i; i = (_DWORD *)((char *)i + ((i[1] + 3) & 0xFFFFFFFC) + 8) )
      ;
    *((_QWORD *)&v17 + 1) = (char *)i - v6 - 4;
  }
  *(_QWORD *)&v15 = this;
  *((_QWORD *)&v15 + 1) = &v17;
  v16 = &v14;
  v18 = &std::_Func_impl_no_alloc<_lambda_13bf9359f506f7ffd00982af1d5f3a49_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::`vftable';
  v19 = v15;
  v20 = &v14;
  v21 = &v18;
  cxl::ValueList::ValueList(&v23, (__int64)&v18, (_QWORD)v17 != 0i64);
  std::function<void (CLUSPROP_ULARGE_INTEGER *)>::~function<void (CLUSPROP_ULARGE_INTEGER *)>((__int64)&v18, v8);
  if ( (struct CLUSPROP_LIST *)v14 == this->CLUSPROP_LIST && !cxl::ValueList::Add(&v23, a2, (union CLUSPROP_SYNTAX)262147) )
  {
    v9 = std::wstring::wstring(v22, L"The property name could not be added");
    *(_QWORD *)&v15 = 13i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v15, (__int64)v9);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  if ( !cxl::ValueList::Add(&v23, **(union _LARGE_INTEGER **)a3, (union CLUSPROP_SYNTAX)(*((union CLUSPROP_SYNTAX **)a3 + 1))->dw) )
  {
    v13 = std::wstring::wstring(v22, L"The property value could not be added");
    *(_QWORD *)&v15 = 13i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v15, (__int64)v13);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  if ( (struct CLUSPROP_LIST *)v14 == this->CLUSPROP_LIST )
  {
    v11 = std::make_pair<std::wstring const &,cxl::PropertyList::Property &>((__int64)v22, a2, &v17);
    std::map<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>>::insert<std::pair<std::wstring,cxl::PropertyList::Property>,0>((__int64 *)this, (__int64)&v15, v11);
    std::wstring::_Tidy_deallocate((__int64)v22);
    ++*(_DWORD *)this->field_10;
    v10 = BYTE8(v15);
  }
  else
  {
    v10 = 1;
  }
  cxl::ValueList::~ValueList(&v23);
  return v10;
}


==========

FUNCTION: ??$AddPropertyT@V_lambda_5d897758465b3281188928bc355bf98e_@@@PropertyList@cxl@@AEAA_NAEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEBV_lambda_5d897758465b3281188928bc355bf98e_@@@Z @ 0x180012B18
----------
bool __fastcall cxl::PropertyList::AddPropertyT<_lambda_5d897758465b3281188928bc355bf98e_>(struct cxl::PropertyList *this, __int64 a2, const struct _lambda_5d897758465b3281188928bc355bf98e_ *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v17 = 0i64;
  std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::find((__int64 *)this, &v14, a2);
  if ( (struct CLUSPROP_LIST *)v14 != this->CLUSPROP_LIST )
  {
    v17 = *(_OWORD *)(v14 + 64);
    v6 = this->field_10;
    for ( i = (_DWORD *)(v6 + *(_QWORD *)(v14 + 72)); *i; i = (_DWORD *)((char *)i + ((i[1] + 3) & 0xFFFFFFFC) + 8) )
      ;
    *((_QWORD *)&v17 + 1) = (char *)i - v6 - 4;
  }
  *(_QWORD *)&v15 = this;
  *((_QWORD *)&v15 + 1) = &v17;
  v16 = &v14;
  v18 = &std::_Func_impl_no_alloc<_lambda_d97f77950fffadeda0a44d074e6b4f77_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::`vftable';
  v19 = v15;
  v20 = &v14;
  v21 = &v18;
  cxl::ValueList::ValueList(&v23, (__int64)&v18, (_QWORD)v17 != 0i64);
  std::function<void (CLUSPROP_ULARGE_INTEGER *)>::~function<void (CLUSPROP_ULARGE_INTEGER *)>((__int64)&v18, v8);
  if ( (struct CLUSPROP_LIST *)v14 == this->CLUSPROP_LIST && !cxl::ValueList::Add(&v23, a2, (union CLUSPROP_SYNTAX)262147) )
  {
    v9 = std::wstring::wstring(v22, L"The property name could not be added");
    *(_QWORD *)&v15 = 13i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v15, (__int64)v9);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  if ( !cxl::ValueList::Add(&v23, *(_QWORD *)a3, (union CLUSPROP_SYNTAX)(*((union CLUSPROP_SYNTAX **)a3 + 1))->dw) )
  {
    v13 = std::wstring::wstring(v22, L"The property value could not be added");
    *(_QWORD *)&v15 = 13i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v15, (__int64)v13);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  if ( (struct CLUSPROP_LIST *)v14 == this->CLUSPROP_LIST )
  {
    v11 = std::make_pair<std::wstring const &,cxl::PropertyList::Property &>((__int64)v22, a2, &v17);
    std::map<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>>::insert<std::pair<std::wstring,cxl::PropertyList::Property>,0>((__int64 *)this, (__int64)&v15, v11);
    std::wstring::_Tidy_deallocate((__int64)v22);
    ++*(_DWORD *)this->field_10;
    v10 = BYTE8(v15);
  }
  else
  {
    v10 = 1;
  }
  cxl::ValueList::~ValueList(&v23);
  return v10;
}


==========

FUNCTION: ??$AddPropertyT@V_lambda_70c4d3f05d475c5512ec25d928cee14c_@@@PropertyList@cxl@@AEAA_NAEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEBV_lambda_70c4d3f05d475c5512ec25d928cee14c_@@@Z @ 0x180012D48
----------
bool __fastcall cxl::PropertyList::AddPropertyT<_lambda_70c4d3f05d475c5512ec25d928cee14c_>(struct cxl::PropertyList *this, __int64 a2, const struct _lambda_70c4d3f05d475c5512ec25d928cee14c_ *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v17 = 0i64;
  std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::find((__int64 *)this, &v14, a2);
  if ( (struct CLUSPROP_LIST *)v14 != this->CLUSPROP_LIST )
  {
    v17 = *(_OWORD *)(v14 + 64);
    v6 = this->field_10;
    for ( i = (_DWORD *)(v6 + *(_QWORD *)(v14 + 72)); *i; i = (_DWORD *)((char *)i + ((i[1] + 3) & 0xFFFFFFFC) + 8) )
      ;
    *((_QWORD *)&v17 + 1) = (char *)i - v6 - 4;
  }
  *(_QWORD *)&v15 = this;
  *((_QWORD *)&v15 + 1) = &v17;
  v16 = &v14;
  v18 = &std::_Func_impl_no_alloc<_lambda_e03fcb33fc3f30c21ab44e5b90795fb4_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::`vftable';
  v19 = v15;
  v20 = &v14;
  v21 = &v18;
  cxl::ValueList::ValueList(&v23, (__int64)&v18, (_QWORD)v17 != 0i64);
  std::function<void (CLUSPROP_ULARGE_INTEGER *)>::~function<void (CLUSPROP_ULARGE_INTEGER *)>((__int64)&v18, v8);
  if ( (struct CLUSPROP_LIST *)v14 == this->CLUSPROP_LIST && !cxl::ValueList::Add(&v23, a2, (union CLUSPROP_SYNTAX)262147) )
  {
    v9 = std::wstring::wstring(v22, L"The property name could not be added");
    *(_QWORD *)&v15 = 13i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v15, (__int64)v9);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  if ( !cxl::ValueList::Add(&v23, **(union _ULARGE_INTEGER **)a3, (union CLUSPROP_SYNTAX)(*((union CLUSPROP_SYNTAX **)a3 + 1))->dw) )
  {
    v13 = std::wstring::wstring(v22, L"The property value could not be added");
    *(_QWORD *)&v15 = 13i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v15, (__int64)v13);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  if ( (struct CLUSPROP_LIST *)v14 == this->CLUSPROP_LIST )
  {
    v11 = std::make_pair<std::wstring const &,cxl::PropertyList::Property &>((__int64)v22, a2, &v17);
    std::map<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>>::insert<std::pair<std::wstring,cxl::PropertyList::Property>,0>((__int64 *)this, (__int64)&v15, v11);
    std::wstring::_Tidy_deallocate((__int64)v22);
    ++*(_DWORD *)this->field_10;
    v10 = BYTE8(v15);
  }
  else
  {
    v10 = 1;
  }
  cxl::ValueList::~ValueList(&v23);
  return v10;
}


==========

FUNCTION: ??$AddPropertyT@V_lambda_866afcf6517d38a17cd38b5b648355d1_@@@PropertyList@cxl@@AEAA_NAEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEBV_lambda_866afcf6517d38a17cd38b5b648355d1_@@@Z @ 0x180012F78
----------
bool __fastcall cxl::PropertyList::AddPropertyT<_lambda_866afcf6517d38a17cd38b5b648355d1_>(struct cxl::PropertyList *this, __int64 a2, const struct _lambda_866afcf6517d38a17cd38b5b648355d1_ *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v17 = 0i64;
  std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::find((__int64 *)this, &v14, a2);
  if ( (struct CLUSPROP_LIST *)v14 != this->CLUSPROP_LIST )
  {
    v17 = *(_OWORD *)(v14 + 64);
    v6 = this->field_10;
    for ( i = (_DWORD *)(v6 + *(_QWORD *)(v14 + 72)); *i; i = (_DWORD *)((char *)i + ((i[1] + 3) & 0xFFFFFFFC) + 8) )
      ;
    *((_QWORD *)&v17 + 1) = (char *)i - v6 - 4;
  }
  *(_QWORD *)&v15 = this;
  *((_QWORD *)&v15 + 1) = &v17;
  v16 = &v14;
  v18 = &std::_Func_impl_no_alloc<_lambda_4ef0909d8e90c100aba481fabd6c4ca0_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::`vftable';
  v19 = v15;
  v20 = &v14;
  v21 = &v18;
  cxl::ValueList::ValueList(&v23, (__int64)&v18, (_QWORD)v17 != 0i64);
  std::function<void (CLUSPROP_ULARGE_INTEGER *)>::~function<void (CLUSPROP_ULARGE_INTEGER *)>((__int64)&v18, v8);
  if ( (struct CLUSPROP_LIST *)v14 == this->CLUSPROP_LIST && !cxl::ValueList::Add(&v23, a2, (union CLUSPROP_SYNTAX)262147) )
  {
    v9 = std::wstring::wstring(v22, L"The property name could not be added");
    *(_QWORD *)&v15 = 13i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v15, (__int64)v9);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  if ( !cxl::ValueList::Add(&v23, **(struct _FILETIME **)a3, (union CLUSPROP_SYNTAX)(*((union CLUSPROP_SYNTAX **)a3 + 1))->dw) )
  {
    v13 = std::wstring::wstring(v22, L"The property value could not be added");
    *(_QWORD *)&v15 = 13i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v15, (__int64)v13);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  if ( (struct CLUSPROP_LIST *)v14 == this->CLUSPROP_LIST )
  {
    v11 = std::make_pair<std::wstring const &,cxl::PropertyList::Property &>((__int64)v22, a2, &v17);
    std::map<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>>::insert<std::pair<std::wstring,cxl::PropertyList::Property>,0>((__int64 *)this, (__int64)&v15, v11);
    std::wstring::_Tidy_deallocate((__int64)v22);
    ++*(_DWORD *)this->field_10;
    v10 = BYTE8(v15);
  }
  else
  {
    v10 = 1;
  }
  cxl::ValueList::~ValueList(&v23);
  return v10;
}


==========

FUNCTION: ??$AddPropertyT@V_lambda_923022901fc03a9890276e9013447403_@@@PropertyList@cxl@@AEAA_NAEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEBV_lambda_923022901fc03a9890276e9013447403_@@@Z @ 0x1800131A8
----------
bool __fastcall cxl::PropertyList::AddPropertyT<_lambda_923022901fc03a9890276e9013447403_>(struct cxl::PropertyList *this, __int64 a2, const struct _lambda_923022901fc03a9890276e9013447403_ *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v17 = 0i64;
  std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::find((__int64 *)this, &v14, a2);
  if ( (struct CLUSPROP_LIST *)v14 != this->CLUSPROP_LIST )
  {
    v17 = *(_OWORD *)(v14 + 64);
    v6 = this->field_10;
    for ( i = (_DWORD *)(v6 + *(_QWORD *)(v14 + 72)); *i; i = (_DWORD *)((char *)i + ((i[1] + 3) & 0xFFFFFFFC) + 8) )
      ;
    *((_QWORD *)&v17 + 1) = (char *)i - v6 - 4;
  }
  *(_QWORD *)&v15 = this;
  *((_QWORD *)&v15 + 1) = &v17;
  v16 = &v14;
  v18 = &std::_Func_impl_no_alloc<_lambda_061c9011cb9bb82bcf658bf6b36e3ac2_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::`vftable';
  v19 = v15;
  v20 = &v14;
  v21 = &v18;
  cxl::ValueList::ValueList(&v23, (__int64)&v18, (_QWORD)v17 != 0i64);
  std::function<void (CLUSPROP_ULARGE_INTEGER *)>::~function<void (CLUSPROP_ULARGE_INTEGER *)>((__int64)&v18, v8);
  if ( (struct CLUSPROP_LIST *)v14 == this->CLUSPROP_LIST && !cxl::ValueList::Add(&v23, a2, (union CLUSPROP_SYNTAX)262147) )
  {
    v9 = std::wstring::wstring(v22, L"The property name could not be added");
    *(_QWORD *)&v15 = 13i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v15, (__int64)v9);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  if ( !cxl::ValueList::Add(&v23, **(_WORD **)a3, (union CLUSPROP_SYNTAX)(*((union CLUSPROP_SYNTAX **)a3 + 1))->dw) )
  {
    v13 = std::wstring::wstring(v22, L"The property value could not be added");
    *(_QWORD *)&v15 = 13i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v15, (__int64)v13);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  if ( (struct CLUSPROP_LIST *)v14 == this->CLUSPROP_LIST )
  {
    v11 = std::make_pair<std::wstring const &,cxl::PropertyList::Property &>((__int64)v22, a2, &v17);
    std::map<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>>::insert<std::pair<std::wstring,cxl::PropertyList::Property>,0>((__int64 *)this, (__int64)&v15, v11);
    std::wstring::_Tidy_deallocate((__int64)v22);
    ++*(_DWORD *)this->field_10;
    v10 = BYTE8(v15);
  }
  else
  {
    v10 = 1;
  }
  cxl::ValueList::~ValueList(&v23);
  return v10;
}


==========

FUNCTION: ??$AddPropertyT@V_lambda_a945c21e4f5b4a83c77c0f81b6ab26fd_@@@PropertyList@cxl@@AEAA_NAEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEBV_lambda_a945c21e4f5b4a83c77c0f81b6ab26fd_@@@Z @ 0x1800133D8
----------
bool __fastcall cxl::PropertyList::AddPropertyT<_lambda_a945c21e4f5b4a83c77c0f81b6ab26fd_>(struct cxl::PropertyList *this, __int64 a2, const struct _lambda_a945c21e4f5b4a83c77c0f81b6ab26fd_ *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v17 = 0i64;
  std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::find((__int64 *)this, &v14, a2);
  if ( (struct CLUSPROP_LIST *)v14 != this->CLUSPROP_LIST )
  {
    v17 = *(_OWORD *)(v14 + 64);
    v6 = this->field_10;
    for ( i = (_DWORD *)(v6 + *(_QWORD *)(v14 + 72)); *i; i = (_DWORD *)((char *)i + ((i[1] + 3) & 0xFFFFFFFC) + 8) )
      ;
    *((_QWORD *)&v17 + 1) = (char *)i - v6 - 4;
  }
  *(_QWORD *)&v15 = this;
  *((_QWORD *)&v15 + 1) = &v17;
  v16 = &v14;
  v18 = &std::_Func_impl_no_alloc<_lambda_3b4ff329403cc3f130f9ace5ccfa8dfa_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::`vftable';
  v19 = v15;
  v20 = &v14;
  v21 = &v18;
  cxl::ValueList::ValueList(&v23, (__int64)&v18, (_QWORD)v17 != 0i64);
  std::function<void (CLUSPROP_ULARGE_INTEGER *)>::~function<void (CLUSPROP_ULARGE_INTEGER *)>((__int64)&v18, v8);
  if ( (struct CLUSPROP_LIST *)v14 == this->CLUSPROP_LIST && !cxl::ValueList::Add(&v23, a2, (union CLUSPROP_SYNTAX)262147) )
  {
    v9 = std::wstring::wstring(v22, L"The property name could not be added");
    *(_QWORD *)&v15 = 13i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v15, (__int64)v9);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  if ( !cxl::ValueList::Add(&v23, **(_DWORD **)a3, (union CLUSPROP_SYNTAX)(*((union CLUSPROP_SYNTAX **)a3 + 1))->dw) )
  {
    v13 = std::wstring::wstring(v22, L"The property value could not be added");
    *(_QWORD *)&v15 = 13i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v15, (__int64)v13);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  if ( (struct CLUSPROP_LIST *)v14 == this->CLUSPROP_LIST )
  {
    v11 = std::make_pair<std::wstring const &,cxl::PropertyList::Property &>((__int64)v22, a2, &v17);
    std::map<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>>::insert<std::pair<std::wstring,cxl::PropertyList::Property>,0>((__int64 *)this, (__int64)&v15, v11);
    std::wstring::_Tidy_deallocate((__int64)v22);
    ++*(_DWORD *)this->field_10;
    v10 = BYTE8(v15);
  }
  else
  {
    v10 = 1;
  }
  cxl::ValueList::~ValueList(&v23);
  return v10;
}


==========

FUNCTION: ??$AddPropertyT@V_lambda_dc7f9f5f34eab37b60d6e390d88b50ff_@@@PropertyList@cxl@@AEAA_NAEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEBV_lambda_dc7f9f5f34eab37b60d6e390d88b50ff_@@@Z @ 0x180013608
----------
bool __fastcall cxl::PropertyList::AddPropertyT<_lambda_dc7f9f5f34eab37b60d6e390d88b50ff_>(struct cxl::PropertyList *this, __int64 a2, const struct _lambda_dc7f9f5f34eab37b60d6e390d88b50ff_ *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v17 = 0i64;
  std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::find((__int64 *)this, &v14, a2);
  if ( (struct CLUSPROP_LIST *)v14 != this->CLUSPROP_LIST )
  {
    v17 = *(_OWORD *)(v14 + 64);
    v6 = this->field_10;
    for ( i = (_DWORD *)(v6 + *(_QWORD *)(v14 + 72)); *i; i = (_DWORD *)((char *)i + ((i[1] + 3) & 0xFFFFFFFC) + 8) )
      ;
    *((_QWORD *)&v17 + 1) = (char *)i - v6 - 4;
  }
  *(_QWORD *)&v15 = this;
  *((_QWORD *)&v15 + 1) = &v17;
  v16 = &v14;
  v18 = &std::_Func_impl_no_alloc<_lambda_37f697dbc2d2dfb7d8e4239a9e92d6a3_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::`vftable';
  v19 = v15;
  v20 = &v14;
  v21 = &v18;
  cxl::ValueList::ValueList(&v23, (__int64)&v18, (_QWORD)v17 != 0i64);
  std::function<void (CLUSPROP_ULARGE_INTEGER *)>::~function<void (CLUSPROP_ULARGE_INTEGER *)>((__int64)&v18, v8);
  if ( (struct CLUSPROP_LIST *)v14 == this->CLUSPROP_LIST && !cxl::ValueList::Add(&v23, a2, (union CLUSPROP_SYNTAX)262147) )
  {
    v9 = std::wstring::wstring(v22, L"The property name could not be added");
    *(_QWORD *)&v15 = 13i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v15, (__int64)v9);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  if ( !cxl::ValueList::Add(&v23, **(_DWORD **)a3, (union CLUSPROP_SYNTAX)(*((union CLUSPROP_SYNTAX **)a3 + 1))->dw) )
  {
    v13 = std::wstring::wstring(v22, L"The property value could not be added");
    *(_QWORD *)&v15 = 13i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v15, (__int64)v13);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  if ( (struct CLUSPROP_LIST *)v14 == this->CLUSPROP_LIST )
  {
    v11 = std::make_pair<std::wstring const &,cxl::PropertyList::Property &>((__int64)v22, a2, &v17);
    std::map<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>>::insert<std::pair<std::wstring,cxl::PropertyList::Property>,0>((__int64 *)this, (__int64)&v15, v11);
    std::wstring::_Tidy_deallocate((__int64)v22);
    ++*(_DWORD *)this->field_10;
    v10 = BYTE8(v15);
  }
  else
  {
    v10 = 1;
  }
  cxl::ValueList::~ValueList(&v23);
  return v10;
}


==========

FUNCTION: ??$AddPropertyT@V_lambda_e669adfef6fcbdcf83b7321db5e4d706_@@@PropertyList@cxl@@AEAA_NAEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEBV_lambda_e669adfef6fcbdcf83b7321db5e4d706_@@@Z @ 0x180013838
----------
bool __fastcall cxl::PropertyList::AddPropertyT<_lambda_e669adfef6fcbdcf83b7321db5e4d706_>(struct cxl::PropertyList *this, __int64 a2, const struct _lambda_e669adfef6fcbdcf83b7321db5e4d706_ *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v17 = 0i64;
  std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::find((__int64 *)this, &v14, a2);
  if ( (struct CLUSPROP_LIST *)v14 != this->CLUSPROP_LIST )
  {
    v17 = *(_OWORD *)(v14 + 64);
    v6 = this->field_10;
    for ( i = (_DWORD *)(v6 + *(_QWORD *)(v14 + 72)); *i; i = (_DWORD *)((char *)i + ((i[1] + 3) & 0xFFFFFFFC) + 8) )
      ;
    *((_QWORD *)&v17 + 1) = (char *)i - v6 - 4;
  }
  *(_QWORD *)&v15 = this;
  *((_QWORD *)&v15 + 1) = &v17;
  v16 = &v14;
  v18 = &std::_Func_impl_no_alloc<_lambda_b2090a35b16344f94b4d1150e607dfd3_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::`vftable';
  v19 = v15;
  v20 = &v14;
  v21 = &v18;
  cxl::ValueList::ValueList(&v23, (__int64)&v18, (_QWORD)v17 != 0i64);
  std::function<void (CLUSPROP_ULARGE_INTEGER *)>::~function<void (CLUSPROP_ULARGE_INTEGER *)>((__int64)&v18, v8);
  if ( (struct CLUSPROP_LIST *)v14 == this->CLUSPROP_LIST && !cxl::ValueList::Add(&v23, a2, (union CLUSPROP_SYNTAX)262147) )
  {
    v9 = std::wstring::wstring(v22, L"The property name could not be added");
    *(_QWORD *)&v15 = 13i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v15, (__int64)v9);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  if ( !cxl::ValueList::Add(&v23, *(_QWORD *)a3, (union CLUSPROP_SYNTAX)(*((union CLUSPROP_SYNTAX **)a3 + 1))->dw) )
  {
    v13 = std::wstring::wstring(v22, L"The property value could not be added");
    *(_QWORD *)&v15 = 13i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v15, (__int64)v13);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  if ( (struct CLUSPROP_LIST *)v14 == this->CLUSPROP_LIST )
  {
    v11 = std::make_pair<std::wstring const &,cxl::PropertyList::Property &>((__int64)v22, a2, &v17);
    std::map<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>>::insert<std::pair<std::wstring,cxl::PropertyList::Property>,0>((__int64 *)this, (__int64)&v15, v11);
    std::wstring::_Tidy_deallocate((__int64)v22);
    ++*(_DWORD *)this->field_10;
    v10 = BYTE8(v15);
  }
  else
  {
    v10 = 1;
  }
  cxl::ValueList::~ValueList(&v23);
  return v10;
}


==========

FUNCTION: ??0PropertyData@cxl@@QEAA@QEAUCLUSPROP_SZ@@AEBUValueData@1@PEAVPropertyList@1@@Z @ 0x180014E74
----------
struct cxl::PropertyData *__fastcall cxl::PropertyData::PropertyData(struct cxl::PropertyData *this, struct CLUSPROP_SZ *const a2, const struct cxl::ValueData *a3, struct cxl::PropertyList *a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  cxl::ValueData::ValueData((struct cxl::ValueData *)this, a3);
  *((_QWORD *)this + 2) = a2;
  *(_OWORD *)((char *)this + 24) = 0i64;
  *((_QWORD *)this + 5) = 0i64;
  *((_QWORD *)this + 6) = 7i64;
  *((_WORD *)this + 12) = 0;
  *((_QWORD *)this + 7) = a4;
  v7 = (_DWORD *)*((_QWORD *)this + 2);
  if ( !v7 || *v7 != 262147 || !v7[1] )
    return this;
  v8 = v7 + 2;
  v9 = -1i64;
  do
    ++v9;
  while ( v8[v9] );
  std::wstring::assign((__int64)this + 24, v8, v9);
  return this;
}


==========

FUNCTION: ??0PropertyList@cxl@@QEAA@QEAUCLUSPROP_LIST@@_K@Z @ 0x180014F08
----------
struct cxl::PropertyList *__fastcall cxl::PropertyList::PropertyList(struct cxl::PropertyList *this, struct CLUSPROP_LIST *const a2, unsigned __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v8[1] = (struct cxl::ErrorCode)this;
  std::map<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>>::map<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>>(this);
  this->field_10 = 0i64;
  this->unknown_struct_18 = 0i64;
  this->map_20 = 0i64;
  if ( a2 && !a3 || a3 && !a2 )
  {
    v6 = std::wstring::wstring(v9, L"The supplied property list or size is not valid");
    v8[0].value_00 = 87;
    v8[0].code_type_04 = 0;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, v8, (__int64)v6);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  cxl::PropertyList::BuildListFromBuffer(this, a2, a3);
  return this;
}


==========

FUNCTION: ??0PropertyList@cxl@@QEAA@XZ @ 0x180014FD4
----------
struct cxl::PropertyList *__fastcall cxl::PropertyList::PropertyList(struct cxl::PropertyList *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::map<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>>::map<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>>(this);
  this->field_10 = 0i64;
  this->unknown_struct_18 = 0i64;
  this->map_20 = 0i64;
  cxl::PropertyList::SetEmptyList(this);
  return this;
}


==========

FUNCTION: ??0property_iterator@PropertyList@cxl@@QEAA@AEBV?$_Tree_const_iterator@V?$_Tree_val@U?$_Tree_simple_types@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@@std@@@std@@@std@@0PEAV12@_N@Z @ 0x180015270
----------
struct cxl::PropertyList::property_iterator *__fastcall cxl::PropertyList::property_iterator::property_iterator(struct cxl::PropertyList::property_iterator *this, __int64 a2, __int64 a3, struct cxl::PropertyList *a4, bool a5)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_OWORD *)&v11.buffer_ptr_00 = 0i64;
  v9 = cxl::ValueData::ValueData(&v12, (__int64)&v11);
  cxl::PropertyData::PropertyData(this, 0i64, v9, 0i64);
  if ( v12.unknown_08 )
    std::_Ref_count_base::_Decref((std::_Ref_count_base *)v12.unknown_08);
  *(_OWORD *)&v11.buffer_ptr_00 = 0i64;
  v11.flag_10 = 0;
  cxl::ValueList::value_iterator::value_iterator((struct cxl::PropertyList::property_iterator *)((char *)this + 64), 0i64, &v11);
  *((_QWORD *)this + 14) = *(_QWORD *)a2;
  *((_QWORD *)this + 15) = *(_QWORD *)a3;
  *((_QWORD *)this + 16) = a4;
  *((_BYTE *)this + 136) = 0;
  cxl::PropertyList::property_iterator::initialize(this);
  return this;
}


==========

FUNCTION: ??0property_iterator@PropertyList@cxl@@QEAA@XZ @ 0x180015330
----------
struct cxl::PropertyList::property_iterator *__fastcall cxl::PropertyList::property_iterator::property_iterator(struct cxl::PropertyList::property_iterator *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v5 = 0i64;
  v2 = cxl::ValueData::ValueData(&v6, (__int64)&v5);
  cxl::PropertyData::PropertyData(this, 0i64, v2, 0i64);
  if ( v6.unknown_08 )
    std::_Ref_count_base::_Decref((std::_Ref_count_base *)v6.unknown_08);
  cxl::ValueList::value_iterator::value_iterator((struct cxl::PropertyList::property_iterator *)((char *)this + 64), v3);
  *((_QWORD *)this + 14) = 0i64;
  *((_QWORD *)this + 15) = 0i64;
  *((_BYTE *)this + 136) = 0;
  cxl::PropertyList::property_iterator::initialize(this);
  return this;
}


==========

FUNCTION: ??1PropertyList@cxl@@QEAA@XZ @ 0x180015524
----------
void __fastcall cxl::PropertyList::~PropertyList(struct cxl::PropertyList *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::vector<unsigned char>::_Tidy((__int64)&this->field_10);
  std::_Tree_val<std::_Tree_simple_types<std::pair<std::wstring const,cxl::PropertyList::Property>>>::_Erase_head<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>((void **)&this->CLUSPROP_LIST, (__int64)this);
}


==========

FUNCTION: ??4PropertyData@cxl@@QEAAAEAV01@$$QEAV01@@Z @ 0x180015758
----------
__int64 *__fastcall cxl::PropertyData::operator=(__int64 *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::shared_ptr<mi::MiApplication>::operator=(a1);
  a1[2] = *(_QWORD *)(a2 + 16);
  std::wstring::operator=(a1 + 3, a2 + 24);
  a1[7] = *(_QWORD *)(a2 + 56);
  return a1;
}


==========

FUNCTION: ??4PropertyList@cxl@@QEAAAEAV01@$$QEBV01@@Z @ 0x1800157A0
----------
__int64 __fastcall cxl::PropertyList::operator=(__int64 a1, _QWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( cxl::PropertyList::operator==((__int64)a2, a1) )
    return a1;
  if ( (_QWORD *)a1 != a2 )
  {
    std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::clear((_QWORD *)a1);
    std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::_Copy<0>((_QWORD *)a1, a2);
  }
  std::vector<unsigned char>::operator=((void **)(a1 + 16), (__int64)(a2 + 2));
  return a1;
}


==========

FUNCTION: ??4property_iterator@PropertyList@cxl@@QEAAAEAV012@AEBV012@@Z @ 0x180015850
----------
__int64 __fastcall cxl::PropertyList::property_iterator::operator=(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)(a1 + 112) = *(_QWORD *)(a2 + 112);
  *(_QWORD *)(a1 + 64) = *(_QWORD *)(a2 + 64);
  std::shared_ptr<mi::MiApplication>::operator=((__int64 *)(a1 + 72));
  *(_QWORD *)(a1 + 88) = *(_QWORD *)(a2 + 88);
  *(_QWORD *)(a1 + 96) = *(_QWORD *)(a2 + 96);
  std::shared_ptr<mi::MiApplication>::operator=((__int64 *)a1);
  *(_QWORD *)(a1 + 16) = *(_QWORD *)(a2 + 16);
  std::wstring::operator=(a1 + 24, a2 + 24);
  *(_QWORD *)(a1 + 56) = *(_QWORD *)(a2 + 56);
  *(_QWORD *)(a1 + 128) = *(_QWORD *)(a2 + 128);
  *(_BYTE *)(a1 + 136) = *(_BYTE *)(a2 + 136);
  *(_QWORD *)(a1 + 120) = *(_QWORD *)(a2 + 120);
  return a1;
}


==========

FUNCTION: ??8Property@PropertyList@cxl@@QEBA_NAEBU012@@Z @ 0x1800158EC
----------
bool __fastcall cxl::PropertyList::Property::operator==(_QWORD *a1, _QWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return *a1 == *a2 && a1[1] == a2[1];
}


==========

FUNCTION: ??8PropertyList@cxl@@QEBA_NAEBV01@@Z @ 0x18001590C
----------
char __fastcall cxl::PropertyList::operator==(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = 0;
  if ( !std::operator==<unsigned char,std::allocator<unsigned char>>(a1 + 16, a2 + 16) || *(_QWORD *)(a1 + 8) != *(_QWORD *)(a2 + 8) )
    return v4;
  v5 = *(__int64 **)a2;
  v6 = *(__int64 **)a1;
  v7 = *v5;
  v8 = **(_QWORD **)a1;
  v10 = v8;
  v11 = v7;
  while ( (__int64 *)v8 != v6 )
  {
    if ( !std::operator==<std::wstring const,cxl::PropertyList::Property,std::wstring const,cxl::PropertyList::Property>(v8 + 32, v7 + 32) )
      return v4;
    std::_Tree_unchecked_const_iterator<std::_Tree_val<std::_Tree_simple_types<std::pair<std::wstring const,cxl::PropertyList::Property>>>,std::_Iterator_base0>::operator++(&v10);
    std::_Tree_unchecked_const_iterator<std::_Tree_val<std::_Tree_simple_types<std::pair<std::wstring const,cxl::PropertyList::Property>>>,std::_Iterator_base0>::operator++(&v11);
    v8 = v10;
    v7 = v11;
  }
  return 1;
}


==========

FUNCTION: ??8property_iterator@PropertyList@cxl@@QEBA_NAEBV012@@Z @ 0x1800159A8
----------
bool __fastcall cxl::PropertyList::property_iterator::operator==(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return *(_QWORD *)(a1 + 64) == *(_QWORD *)(a2 + 64) && *(_QWORD *)(a1 + 112) == *(_QWORD *)(a2 + 112);
}


==========

FUNCTION: ?BuildListFromBuffer@PropertyList@cxl@@AEAAXQEAUCLUSPROP_LIST@@_K@Z @ 0x18001704C
----------
void __fastcall cxl::PropertyList::BuildListFromBuffer(struct cxl::PropertyList *this, struct CLUSPROP_LIST *const a2, unsigned __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a2 && a3 )
  {
    v3 = this;
    cxl::PropertyList::BuildListFromBuffer__lambda_83667e7cab82fc438ddf9871c76c843c___((__int64 *)this, (__int64)a2, a3, (__int64)&v3);
  }
  else
  {
    cxl::PropertyList::SetEmptyList(this);
  }
}


==========

FUNCTION: ?SetEmptyList@PropertyList@cxl@@AEAAXXZ @ 0x1800172A0
----------
void __fastcall cxl::PropertyList::SetEmptyList(struct cxl::PropertyList *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::clear(this);
  v3 = (char *)this->unknown_struct_18;
  v4 = this->field_10;
  v5 = (unsigned __int64)&v3[-v4];
  if ( (unsigned __int64)&v3[-v4] > 8 )
  {
    v6 = (char *)(v4 + 8);
LABEL_7:
    this->unknown_struct_18 = v6;
    goto LABEL_8;
  }
  if ( v5 < 8 )
  {
    if ( (unsigned __int64)this->map_20 - v4 >= 8 )
    {
      v7 = 8 - v5;
      memset_0(this->unknown_struct_18, 0, 8 - v5);
      v6 = &v3[v7];
      goto LABEL_7;
    }
    std::vector<unsigned char>::_Resize_reallocate<std::_Value_init_tag>((const void **)&this->field_10, 8ui64, v2);
  }
LABEL_8:
  *(_DWORD *)this->field_10 = 0;
  *(_DWORD *)(this->field_10 + 4) = 0;
}


==========

FUNCTION: ?SetToDefaultValue@PropertyList@cxl@@QEAAXAEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@Z @ 0x180017330
----------
void __fastcall cxl::PropertyList::SetToDefaultValue(struct cxl::PropertyList *this, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::find((__int64 *)this, &v4, a2);
  if ( (struct CLUSPROP_LIST *)v4 == this->CLUSPROP_LIST )
  {
    v3 = std::wstring::wstring(v5, L"The property not found");
    v4 = 2i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v4, (__int64)v3);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  cxl::PropertyList::default_value(this, (struct CLUSPROP_SZ *const)(*(_QWORD *)(v4 + 64) + this->field_10));
}


==========

FUNCTION: ?UpdateInternalPointers@PropertyList@cxl@@AEAAXH_K@Z @ 0x1800173D8
----------
void __fastcall cxl::PropertyList::UpdateInternalPointers(struct cxl::PropertyList *this, int a2, unsigned __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = a2;
  v6 = *(_QWORD *)&this->CLUSPROP_LIST->nPropertyCount;
  v9 = v6;
  while ( (struct CLUSPROP_LIST *)v6 != this->CLUSPROP_LIST )
  {
    v7 = *(_QWORD *)(v6 + 64);
    if ( v7 >= a3 )
      *(_QWORD *)(v6 + 64) = v7 + v4;
    v8 = *(_QWORD *)(v6 + 72);
    if ( v8 >= a3 )
      *(_QWORD *)(v6 + 72) = v4 + v8;
    std::_Tree_unchecked_const_iterator<std::_Tree_val<std::_Tree_simple_types<std::pair<std::wstring const,cxl::PropertyList::Property>>>,std::_Iterator_base0>::operator++(&v9);
    v6 = v9;
  }
}


==========

FUNCTION: ?default_value@PropertyList@cxl@@AEAAXQEAUCLUSPROP_SZ@@@Z @ 0x180017E24
----------
void __fastcall cxl::PropertyList::default_value(struct cxl::PropertyList *this, struct CLUSPROP_SZ *const a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = (struct CLUSPROP_VALUE *)&a2->value_08[(a2->cbLength_04 + 3) & 0xFFFFFFFC];
  v4 = (v3->cbLength_04 + 3) & 0xFFFFFFFC;
  v5 = &v3->value_08[v4];
  while ( *(_DWORD *)v5 )
  {
    v6 = ((*((_DWORD *)v5 + 1) + 3) & 0xFFFFFFFC) + 8i64;
    v5 += v6;
    LODWORD(v4) = v6 + v4;
  }
  cxl::PropertyList::resize(this, v3, -(int)v4)->cbLength_04 = 0;
}


==========

FUNCTION: ?end@PropertyList@cxl@@QEBA?BVproperty_iterator@12@XZ @ 0x180017E88
----------
struct cxl::PropertyList::property_iterator *__fastcall cxl::PropertyList::end(struct cxl::PropertyList *a1, struct cxl::PropertyList::property_iterator *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v6 = a2;
  v6 = a1->CLUSPROP_LIST;
  v5 = v6;
  cxl::PropertyList::property_iterator::property_iterator(a2, (__int64)&v5, (__int64)&v6, a1, v4);
  return a2;
}


==========

FUNCTION: ?find@PropertyList@cxl@@QEAA?AVproperty_iterator@12@AEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@Z @ 0x180017F8C
----------
struct cxl::PropertyList::property_iterator *__fastcall cxl::PropertyList::find(struct cxl::PropertyList *a1, struct cxl::PropertyList::property_iterator *this, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::find((__int64 *)a1, &v7, a3);
  if ( (struct CLUSPROP_LIST *)v7 == a1->CLUSPROP_LIST )
  {
    cxl::PropertyList::end(a1, this);
  }
  else
  {
    v8 = (__int64)a1->CLUSPROP_LIST;
    cxl::PropertyList::property_iterator::property_iterator(this, (__int64)&v7, (__int64)&v8, a1, v6);
  }
  return this;
}


==========

FUNCTION: ?get_item@property_iterator@PropertyList@cxl@@AEAAXXZ @ 0x1800184C0
----------
void __fastcall cxl::PropertyList::property_iterator::get_item(struct cxl::PropertyList::property_iterator *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = cxl::ValueList::value_iterator::value_iterator((struct cxl::ValueList::value_iterator *)v12, v1);
  v4 = *((_QWORD *)this + 8);
  v5 = *(_QWORD *)v3;
  if ( v13 )
    std::_Ref_count_base::_Decref(v13);
  if ( v5 != v4 )
  {
    v6 = (struct cxl::PropertyList *)*((_QWORD *)this + 16);
    v7 = *(_QWORD *)(*((_QWORD *)this + 14) + 64i64);
    v8 = v6->field_10;
    v9 = (unsigned __int64)v6->unknown_struct_18 - v8;
    v10 = (struct CLUSPROP_SZ *)(v7 + v8);
    if ( v9 < v7 + 8 )
      v10 = 0i64;
    v11 = cxl::PropertyData::PropertyData((struct cxl::PropertyData *)v12, v10, (const struct cxl::ValueData *)((char *)this + 72), v6);
    cxl::PropertyData::operator=((__int64 *)this, (__int64)v11);
    cxl::PropertyData::~PropertyData((struct cxl::PropertyData *)v12);
  }
}


==========

FUNCTION: ?initialize@property_iterator@PropertyList@cxl@@AEAAXXZ @ 0x180018BC4
----------
void __fastcall cxl::PropertyList::property_iterator::initialize(struct cxl::PropertyList::property_iterator *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( *((_QWORD *)this + 14) != *((_QWORD *)this + 15) )
  {
    cxl::PropertyList::property_iterator::next_value(this);
    cxl::PropertyList::property_iterator::get_item(this);
  }
}


==========

FUNCTION: ?next_item@property_iterator@PropertyList@cxl@@AEAAXXZ @ 0x180018E48
----------
void __fastcall cxl::PropertyList::property_iterator::next_item(struct cxl::PropertyList::property_iterator *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_OWORD *)v16 = 0i64;
  v2 = cxl::ValueData::ValueData(&v17, (__int64)v16);
  v3 = cxl::PropertyData::PropertyData((struct cxl::PropertyData *)v18, 0i64, v2, 0i64);
  cxl::PropertyData::operator=((__int64 *)this, (__int64)v3);
  cxl::PropertyData::~PropertyData((struct cxl::PropertyData *)v18);
  if ( v17.unknown_08 )
    std::_Ref_count_base::_Decref((std::_Ref_count_base *)v17.unknown_08);
  v6 = (__int64 *)((char *)this + 64);
  v7 = 1;
  v8 = *(_QWORD *)cxl::ValueList::value_iterator::value_iterator((struct cxl::ValueList::value_iterator *)v18, v4) != *((_QWORD *)this + 8) || *((_QWORD *)this + 14) != *((_QWORD *)this + 15);
  if ( v19 )
    std::_Ref_count_base::_Decref(v19);
  if ( v8 )
  {
    v10 = *(_QWORD *)cxl::ValueList::value_iterator::value_iterator((struct cxl::ValueList::value_iterator *)v18, v5);
    v11 = *v6;
    if ( v19 )
      std::_Ref_count_base::_Decref(v19);
    if ( v10 != v11 )
      cxl::ValueList::value_iterator::next_value((struct cxl::PropertyList::property_iterator *)((char *)this + 64));
    v12 = 0;
    if ( !*((_BYTE *)this + 136) && (v13 = cxl::ValueList::value_iterator::value_iterator((struct cxl::ValueList::value_iterator *)v18, v9), v12 = 2, *v6 != *(_QWORD *)v13) || *((_QWORD *)this + 14) == *((_QWORD *)this + 15) || (std::_Tree_unchecked_const_iterator<std::_Tree_val<std::_Tree_simple_types<std::pair<std::wstring const,cxl::PropertyList::Property>>>,std::_Iterator_base0>::operator++((_QWORD *)this + 14), *v14 == *((_QWORD *)this + 15)) )
      v7 = 0;
    if ( v12 && v19 )
      std::_Ref_count_base::_Decref(v19);
    if ( v7 )
    {
      cxl::PropertyList::property_iterator::next_value(this);
    }
    else if ( *((_QWORD *)this + 14) == *((_QWORD *)this + 15) )
    {
      v15 = cxl::ValueList::value_iterator::value_iterator((struct cxl::ValueList::value_iterator *)v18, v9);
      *((_QWORD *)this + 8) = *(_QWORD *)v15;
      std::shared_ptr<mi::MiApplication>::operator=((__int64 *)this + 9);
      *((_QWORD *)this + 11) = *((_QWORD *)v15 + 3);
      *((_QWORD *)this + 12) = *((_QWORD *)v15 + 4);
      if ( v19 )
        std::_Ref_count_base::_Decref(v19);
    }
    cxl::PropertyList::property_iterator::get_item(this);
  }
}


==========

FUNCTION: ?next_value@property_iterator@PropertyList@cxl@@AEAAXXZ @ 0x180019018
----------
void __fastcall cxl::PropertyList::property_iterator::next_value(struct cxl::PropertyList::property_iterator *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = 0;
  v5 = 1;
  if ( !*((_BYTE *)this + 136) )
  {
    if ( *((_QWORD *)this + 14) == *((_QWORD *)this + 15) || (v4 = cxl::ValueList::value_iterator::value_iterator((struct cxl::ValueList::value_iterator *)v11, v1), v3 = 1, *((_QWORD *)this + 8) != *(_QWORD *)v4) )
      v5 = 0;
  }
  if ( (v3 & 1) != 0 && v12 )
    std::_Ref_count_base::_Decref(v12);
  if ( v5 )
  {
    v6 = *((_QWORD *)this + 14);
    v7 = *((_QWORD *)this + 16);
    v11[0] = 0i64;
    v11[1] = v7;
    v8 = *(_QWORD *)(v6 + 72);
    LOBYTE(v12) = 1;
    v9 = cxl::ValueList::value_iterator::value_iterator((struct cxl::ValueList::value_iterator *)v13, v8, (const struct cxl::ValueList::list_accessor *)v11);
    *((_QWORD *)this + 8) = *(_QWORD *)v9;
    std::shared_ptr<mi::MiApplication>::operator=((__int64 *)this + 9);
    *((_QWORD *)this + 11) = *((_QWORD *)v9 + 3);
    v10 = v14;
    *((_QWORD *)this + 12) = *((_QWORD *)v9 + 4);
    if ( v10 )
      std::_Ref_count_base::_Decref(v10);
  }
}


==========

FUNCTION: ?resize@PropertyList@cxl@@AEAAPEAUCLUSPROP_VALUE@@QEAU3@H@Z @ 0x1800191B8
----------
struct CLUSPROP_VALUE *__fastcall cxl::PropertyList::resize(struct cxl::PropertyList *this, struct CLUSPROP_VALUE *const a2, int a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = a3;
  v5 = &this->field_10;
  v6 = this->field_10;
  v7 = &a2->value_08[-v6];
  if ( v7 > (char *)this->unknown_struct_18 - v6 )
  {
    v10 = std::wstring::wstring(v13, L"The position of the value is not within the buffer");
    v12.value_00 = 13;
    v12.code_type_04 = 0;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, &v12, (__int64)v10);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  v8 = a2->value_08;
  if ( (int)v3 /*signed*/<= 0 )
  {
    std::vector<unsigned char>::erase((__int64)v5, &v12, v8, &a2->value_08[-(int)v3]);
  }
  else
  {
    v11[0] = 0;
    std::vector<unsigned char>::insert((void **)v5, &v12, v8, v3, v11);
  }
  cxl::PropertyList::UpdateInternalPointers(this, v3, (unsigned __int64)(v7 - 7));
  return (struct CLUSPROP_VALUE *)&v7[*v5 - 8];
}


==========

Source file: Microsoft.FailoverClusters.FrameworkSupport
Generated time: 20260130_193545
==========

FUNCTION: GetDwordProperty @ 0x18000E510
----------
__int64 __fastcall GetDwordProperty(_DWORD *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = a2;
  return ExecuteOnIterator_PropList::ListHandle_cxl::PropertyList___lambda_6bd7d65065ccc55fa2a4354bff4a4e86___(a1, a2, (__int64)&v3);
}


==========

