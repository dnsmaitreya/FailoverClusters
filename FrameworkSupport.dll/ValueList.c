Source file: Microsoft.FailoverClusters.FrameworkSupport
Generated time: 20260130_192639
==========

FUNCTION: ??1value_iterator@ValueList@cxl@@QEAA@XZ @ 0x18000B944
----------
void __fastcall cxl::ValueList::value_iterator::~value_iterator(struct cxl::ValueList::value_iterator *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v1 = (std::_Ref_count_base *)*((_QWORD *)this + 2);
  if ( v1 )
    std::_Ref_count_base::_Decref(v1);
}


==========

FUNCTION: ??$UpdateValue@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@?$ValueItem@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@ValueList@cxl@@IEAAPEAUCLUSPROP_VALUE@@AEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@_K@Z @ 0x180014010
----------
__int64 __fastcall cxl::ValueList::ValueItem<std::wstring>::UpdateValue<std::wstring>(__int64 a1, __int64 a2, rsize_t a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v6 = *(struct CLUSPROP_VALUE **)(a1 + 8);
  v7 = (a3 + 3) & 0xFFFFFFFFFFFFFFFCui64;
  if ( v7 != ((v6->cbLength_04 + 3) & 0xFFFFFFFC) )
    *(_QWORD *)(a1 + 8) = cxl::ValueList::list_accessor::resize((struct cxl::ValueList::list_accessor *)(a1 + 16), v6, ((a3 + 3) & 0xFFFFFFFC) - ((v6->cbLength_04 + 3) & 0xFFFFFFFC));
  v8 = std::_String_val<std::_Simple_types<wchar_t>>::_Myptr(a2);
  memcpy_s((void *const)(v9 + 8), v7, v8, a3);
  if ( v7 > a3 )
    memset_0((void *)(a3 + *(_QWORD *)(a1 + 8) + 8i64), 0, v7 - a3);
  *(_DWORD *)(*(_QWORD *)(a1 + 8) + 4i64) = a3;
  return *(_QWORD *)(a1 + 8);
}


==========

FUNCTION: ??$UpdateValue@V?$vector@EV?$allocator@E@std@@@std@@@?$ValueItem@V?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@@ValueList@cxl@@IEAAPEAUCLUSPROP_VALUE@@AEBV?$vector@EV?$allocator@E@std@@@std@@_K@Z @ 0x1800140D8
----------
__int64 __fastcall cxl::ValueList::ValueItem<std::vector<std::wstring>>::UpdateValue<std::vector<unsigned char>>(__int64 a1, const void *const *a2, rsize_t a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v6 = *(struct CLUSPROP_VALUE **)(a1 + 8);
  v7 = (a3 + 3) & 0xFFFFFFFFFFFFFFFCui64;
  if ( v7 != ((v6->cbLength_04 + 3) & 0xFFFFFFFC) )
  {
    v6 = cxl::ValueList::list_accessor::resize((struct cxl::ValueList::list_accessor *)(a1 + 16), v6, ((a3 + 3) & 0xFFFFFFFC) - ((v6->cbLength_04 + 3) & 0xFFFFFFFC));
    *(_QWORD *)(a1 + 8) = v6;
  }
  memcpy_s(v6->value_08, v7, *a2, a3);
  if ( v7 > a3 )
    memset_0((void *)(a3 + *(_QWORD *)(a1 + 8) + 8i64), 0, v7 - a3);
  *(_DWORD *)(*(_QWORD *)(a1 + 8) + 4i64) = a3;
  return *(_QWORD *)(a1 + 8);
}


==========

FUNCTION: ??$UpdateValue@V?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@@?$ValueItem@V?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@@ValueList@cxl@@IEAAPEAUCLUSPROP_VALUE@@AEBV?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@_K@Z @ 0x180014198
----------
__int64 __fastcall cxl::ValueList::ValueItem<std::vector<std::wstring>>::UpdateValue<std::vector<std::wstring>>(__int64 a1, __int64 *a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v21 = a3;
  v5 = 0i64;
  v17 = 0i64;
  v6 = 0i64;
  v18 = 0i64;
  v7 = 0i64;
  v19 = 0i64;
  v8 = 0i64;
  v9 = *a2;
  while ( v9 != a2[1] )
  {
    v10 = *(_QWORD *)(v9 + 16);
    if ( v10 )
    {
      v11 = (unsigned __int64)&v6[2 * v10 + 2 - (_QWORD)v5];
      if ( v11 < v6 - v5 )
      {
        v6 = &v5[v11];
        goto LABEL_10;
      }
      if ( v11 > v6 - v5 )
      {
        if ( v11 <= v7 - (__int64)v5 )
        {
          v12 = v11 - (v6 - v5);
          memset_0(v6, 0, v12);
          v6 += v12;
LABEL_10:
          v18 = v6;
        }
        else
        {
          std::vector<unsigned char>::_Resize_reallocate<std::_Value_init_tag>((const void **)&v17, v11, (size_t)v5);
          v6 = v18;
        }
      }
      v13 = std::_String_val<std::_Simple_types<wchar_t>>::_Myptr(v9);
      memcpy_s((void *const)(v14 + v8), (const rsize_t)&v6[-v14 - v8], v13, 2i64 * *(_QWORD *)(v9 + 16) + 2);
      goto LABEL_13;
    }
    LOBYTE(v21) = 0;
    std::vector<unsigned char>::insert((void **)&v17, &v20, v6, 2ui64, (unsigned __int8 *)&v21);
LABEL_13:
    v8 += 2i64 * *(_QWORD *)(v9 + 16) + 2;
    v9 += 32i64;
    v7 = v19;
    v6 = v18;
    v5 = v17;
  }
  LOBYTE(v21) = 0;
  std::vector<unsigned char>::insert((void **)&v17, &v20, v6, 2ui64, (unsigned __int8 *)&v21);
  v15 = cxl::ValueList::ValueItem<std::vector<std::wstring>>::UpdateValue<std::vector<unsigned char>>(a1, (const void *const *)&v17, v18 - v17);
  std::vector<unsigned char>::_Tidy((__int64)&v17);
  return v15;
}


==========

FUNCTION: ??0ValueData@cxl@@QEAA@AEBU01@@Z @ 0x180015010
----------
struct cxl::ValueData *__fastcall cxl::ValueData::ValueData(struct cxl::ValueData *this, const struct cxl::ValueData *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  this->shared_ptr_00 = 0i64;
  this->unknown_08 = 0i64;
  std::shared_ptr<mi::MiApplication>::operator=((__int64 *)this);
  return this;
}


==========

FUNCTION: ??0ValueData@cxl@@QEAA@AEBV?$shared_ptr@UIValue@cxl@@@std@@@Z @ 0x180015038
----------
struct cxl::ValueData *__fastcall cxl::ValueData::ValueData(struct cxl::ValueData *this, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::shared_ptr<mi::MiApplication>::shared_ptr<mi::MiApplication>(this);
  return v2;
}


==========

FUNCTION: ??0ValueList@cxl@@AEAA@AEBV?$function@$$A6APEAUCLUSPROP_VALUE@@_K0@Z@std@@W4Flags@ValueListOptions@1@@Z @ 0x180015050
----------
_QWORD *__fastcall cxl::ValueList::ValueList(struct cxl::ValueList *a1, __int64 a2, char a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  a1->vector_00 = 0i64;
  a1->field_8 = 0i64;
  a1->field_10 = 0i64;
  std::function<CLUSPROP_VALUE * (unsigned __int64,unsigned __int64)>::function<CLUSPROP_VALUE * (unsigned __int64,unsigned __int64)>((__int64)&a1->end_mark_18, a2);
  if ( (a3 & 1) == 0 )
    *(_DWORD *)std::_Func_class<CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::operator()((__int64)&a1->end_mark_18, 0i64, 4i64) = 0;
  return &a1->vector_00;
}


==========

FUNCTION: ??0ValueList@cxl@@QEAA@QEBE_K@Z @ 0x1800150B4
----------
struct cxl::ValueList *__fastcall cxl::ValueList::ValueList(struct cxl::ValueList *this, const unsigned __int8 *const a2, unsigned __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v14[2] = (struct cxl::ErrorCode)this;
  this->vector_00 = 0i64;
  this->field_8 = 0i64;
  this->field_10 = 0i64;
  this->field_50 = 0i64;
  if ( a2 && !a3 || a3 && !a2 )
  {
    v6 = std::wstring::wstring(v15, L"The supplied buffer or size is not valid");
    v14[0].value_00 = 87;
    v14[0].code_type_04 = 0;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, v14, (__int64)v6);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  std::vector<unsigned char>::_Assign_counted_range<unsigned char const *>(&this->vector_00, (char *)a2, a3);
  v7 = cxl::ValueList::ParseBuffer(this, a2, a3);
  v9 = this->field_8;
  v10 = this->vector_00;
  v11 = v9 - (unsigned __int64)this->vector_00;
  if ( v7 >= v11 )
  {
    if ( v7 > v11 )
    {
      if ( v7 <= this->field_10 - (__int64)v10 )
      {
        v12 = v7 - v11;
        memset_0((void *)this->field_8, 0, v7 - v11);
        this->field_8 = v9 + v12;
      }
      else
      {
        std::vector<unsigned char>::_Resize_reallocate<std::_Value_init_tag>((const void **)&this->vector_00, v7, v8);
      }
    }
  }
  else
  {
    this->field_8 = (__int64)v10 + v7;
  }
  return this;
}


==========

FUNCTION: cxl::ValueList::ValueList(void) @ 0x1800151D4
----------
struct cxl::ValueList *__fastcall cxl::ValueList::ValueList_void_(struct cxl::ValueList *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  this->vector_00 = 0i64;
  this->field_8 = 0i64;
  this->field_10 = 0i64;
  this->field_50 = 0i64;
  v3 = (_BYTE *)this->field_8;
  v4 = this->vector_00;
  v5 = v3 - (_BYTE *)this->vector_00;
  if ( v5 > 4 )
  {
    v6 = (__int64)v4 + 4;
LABEL_7:
    this->field_8 = v6;
    goto LABEL_8;
  }
  if ( v5 < 4 )
  {
    if ( this->field_10 - (__int64)v4 >= 4ui64 )
    {
      v7 = 4 - v5;
      memset_0(v3, 0, 4 - v5);
      v6 = (__int64)&v3[v7];
      goto LABEL_7;
    }
    std::vector<unsigned char>::_Resize_reallocate<std::_Value_init_tag>((const void **)&this->vector_00, 4ui64, v1);
  }
LABEL_8:
  *(_DWORD *)this->vector_00 = 0;
  return this;
}


==========

FUNCTION: ??0value_iterator@ValueList@cxl@@QEAA@_K@Z @ 0x1800153AC
----------
struct cxl::ValueList::value_iterator *__fastcall cxl::ValueList::value_iterator::value_iterator(struct cxl::ValueList::value_iterator *this, unsigned __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)this = -1i64;
  v4 = 0i64;
  cxl::ValueData::ValueData((struct cxl::ValueData *)((char *)this + 8), (__int64)&v4);
  *((_QWORD *)this + 3) = 0i64;
  *((_QWORD *)this + 4) = 0i64;
  *((_BYTE *)this + 40) = 0;
  cxl::ValueList::value_iterator::initialize(this);
  return this;
}


==========

FUNCTION: ??0value_iterator@ValueList@cxl@@QEAA@_KAEBVlist_accessor@12@@Z @ 0x1800153FC
----------
struct cxl::ValueList::value_iterator *__fastcall cxl::ValueList::value_iterator::value_iterator(struct cxl::ValueList::value_iterator *this, unsigned __int64 a2, const struct cxl::ValueList::list_accessor *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)this = a2;
  v6 = 0i64;
  cxl::ValueData::ValueData((struct cxl::ValueData *)((char *)this + 8), (__int64)&v6);
  *((_QWORD *)this + 3) = *v4;
  *((_QWORD *)this + 4) = v4[1];
  cxl::ValueList::value_iterator::initialize(this);
  return this;
}


==========

FUNCTION: ??1ValueList@cxl@@QEAA@XZ @ 0x18001554C
----------
void __fastcall cxl::ValueList::~ValueList(struct cxl::ValueList *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::function<void (CLUSPROP_ULARGE_INTEGER *)>::~function<void (CLUSPROP_ULARGE_INTEGER *)>((__int64)&this->end_mark_18, v1);
  std::vector<unsigned char>::_Tidy((__int64)this);
}


==========

FUNCTION: ??4ValueList@cxl@@QEAAAEAV01@$$QEAV01@@Z @ 0x1800157FC
----------
_QWORD *__fastcall cxl::ValueList::operator=(_QWORD *a1, _QWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::vector<unsigned char>::operator=(a1, a2);
  v5 = a2 + 3;
  if ( a1 + 3 == v5 )
    return a1;
  std::function<void (CLUSPROP_ULARGE_INTEGER *)>::~function<void (CLUSPROP_ULARGE_INTEGER *)>((__int64)(a1 + 3), v4);
  std::_Func_class<CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::_Reset_move((__int64)(a1 + 3), (__int64)v5);
  return a1;
}


==========

FUNCTION: ??_G?$ValueElement@GUCLUSPROP_WORD@@@ValueList@cxl@@UEAAPEAXI@Z @ 0x180015EA0
----------
_QWORD *__fastcall cxl::ValueList::ValueElement<unsigned short,CLUSPROP_WORD>::`scalar deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &cxl::ValueList::ValueItem<unsigned short>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_G?$ValueElement@JUCLUSPROP_LONG@@@ValueList@cxl@@UEAAPEAXI@Z @ 0x180015EE0
----------
_QWORD *__fastcall cxl::ValueList::ValueElement<long,CLUSPROP_LONG>::`scalar deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &cxl::ValueList::ValueItem<long>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_G?$ValueElement@KUCLUSPROP_DWORD@@@ValueList@cxl@@UEAAPEAXI@Z @ 0x180015F20
----------
_QWORD *__fastcall cxl::ValueList::ValueElement<unsigned long,CLUSPROP_DWORD>::`scalar deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &cxl::ValueList::ValueItem<unsigned long>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_G?$ValueElement@T_LARGE_INTEGER@@UCLUSPROP_LARGE_INTEGER@@@ValueList@cxl@@UEAAPEAXI@Z @ 0x180015F60
----------
_QWORD *__fastcall cxl::ValueList::ValueElement<_LARGE_INTEGER,CLUSPROP_LARGE_INTEGER>::`scalar deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &cxl::ValueList::ValueItem<_LARGE_INTEGER>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_E?$ValueElement@T_ULARGE_INTEGER@@UCLUSPROP_ULARGE_INTEGER@@@ValueList@cxl@@UEAAPEAXI@Z @ 0x180015FA0
----------
_QWORD *__fastcall cxl::ValueList::ValueElement<_ULARGE_INTEGER,CLUSPROP_ULARGE_INTEGER>::`vector deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &cxl::ValueList::ValueItem<_ULARGE_INTEGER>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_E?$ValueElement@U_FILETIME@@UCLUSPROP_FILETIME@@@ValueList@cxl@@UEAAPEAXI@Z @ 0x180015FE0
----------
_QWORD *__fastcall cxl::ValueList::ValueElement<_FILETIME,CLUSPROP_FILETIME>::`vector deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &cxl::ValueList::ValueItem<_FILETIME>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_E?$ValueElement@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UCLUSPROP_SZ@@@ValueList@cxl@@UEAAPEAXI@Z @ 0x180016020
----------
_QWORD *__fastcall cxl::ValueList::ValueElement<std::wstring,CLUSPROP_SZ>::`vector deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &cxl::ValueList::ValueItem<std::wstring>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_E?$ValueElement@V?$vector@EV?$allocator@E@std@@@std@@UCLUSPROP_BINARY@@@ValueList@cxl@@UEAAPEAXI@Z @ 0x180016060
----------
_QWORD *__fastcall cxl::ValueList::ValueElement<std::vector<unsigned char>,CLUSPROP_BINARY>::`vector deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &cxl::ValueList::ValueItem<std::vector<unsigned char>>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_G?$ValueElement@V?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@UCLUSPROP_SZ@@@ValueList@cxl@@UEAAPEAXI@Z @ 0x1800160A0
----------
_QWORD *__fastcall cxl::ValueList::ValueElement<std::vector<std::wstring>,CLUSPROP_SZ>::`scalar deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &cxl::ValueList::ValueItem<std::vector<std::wstring>>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_E?$ValueItem@G@ValueList@cxl@@UEAAPEAXI@Z @ 0x1800160E0
----------
_QWORD *__fastcall cxl::ValueList::ValueItem<unsigned short>::`vector deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &cxl::ValueList::ValueItem<unsigned short>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_E?$ValueItem@J@ValueList@cxl@@UEAAPEAXI@Z @ 0x180016120
----------
_QWORD *__fastcall cxl::ValueList::ValueItem<long>::`vector deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &cxl::ValueList::ValueItem<long>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_E?$ValueItem@K@ValueList@cxl@@UEAAPEAXI@Z @ 0x180016160
----------
_QWORD *__fastcall cxl::ValueList::ValueItem<unsigned long>::`vector deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &cxl::ValueList::ValueItem<unsigned long>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_E?$ValueItem@T_LARGE_INTEGER@@@ValueList@cxl@@UEAAPEAXI@Z @ 0x1800161A0
----------
_QWORD *__fastcall cxl::ValueList::ValueItem<_LARGE_INTEGER>::`vector deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &cxl::ValueList::ValueItem<_LARGE_INTEGER>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_E?$ValueItem@T_ULARGE_INTEGER@@@ValueList@cxl@@UEAAPEAXI@Z @ 0x1800161E0
----------
_QWORD *__fastcall cxl::ValueList::ValueItem<_ULARGE_INTEGER>::`vector deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &cxl::ValueList::ValueItem<_ULARGE_INTEGER>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_E?$ValueItem@U_FILETIME@@@ValueList@cxl@@UEAAPEAXI@Z @ 0x180016220
----------
_QWORD *__fastcall cxl::ValueList::ValueItem<_FILETIME>::`vector deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &cxl::ValueList::ValueItem<_FILETIME>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_G?$ValueItem@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@ValueList@cxl@@UEAAPEAXI@Z @ 0x180016260
----------
_QWORD *__fastcall cxl::ValueList::ValueItem<std::wstring>::`scalar deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &cxl::ValueList::ValueItem<std::wstring>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_E?$ValueItem@V?$vector@EV?$allocator@E@std@@@std@@@ValueList@cxl@@UEAAPEAXI@Z @ 0x1800162A0
----------
_QWORD *__fastcall cxl::ValueList::ValueItem<std::vector<unsigned char>>::`vector deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &cxl::ValueList::ValueItem<std::vector<unsigned char>>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_E?$ValueItem@V?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@@ValueList@cxl@@UEAAPEAXI@Z @ 0x1800162E0
----------
_QWORD *__fastcall cxl::ValueList::ValueItem<std::vector<std::wstring>>::`vector deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &cxl::ValueList::ValueItem<std::vector<std::wstring>>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ?Add@ValueList@cxl@@QEAA_NAEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@TCLUSPROP_SYNTAX@@@Z @ 0x180016554
----------
bool __fastcall cxl::ValueList::Add(struct cxl::ValueList *this, __int64 a2, union CLUSPROP_SYNTAX a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v7.dw = a3.dw;
  if ( ((a3.wFormat - 3) & 0xFFFA) != 0 || a3.wFormat == 7 )
  {
    v6 = std::wstring::wstring(&v10, L"The syntax is not valid for a CLUSPROP_SZ value");
    v8.value_00 = 87;
    v8.code_type_04 = 0;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, &v8, (__int64)v6);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  v9 = 2i64 * *(_QWORD *)(a2 + 16) + 2;
  *(_QWORD *)&v10 = &v9;
  *((_QWORD *)&v10 + 1) = &v7;
  v11 = a2;
  v12 = &std::_Func_impl_no_alloc<_lambda_dee7ce9a0e8b9a680e058a673693a19f_,void,CLUSPROP_SZ *>::`vftable';
  v13 = v10;
  v14 = a2;
  v15 = &v12;
  v3 = cxl::ValueList::alloc(this, (v9 + 11) & 0xFFFFFFFFFFFFFFFCui64);
  std::_Func_class<void,CLUSPROP_SZ *>::operator()((__int64)&v12, (__int64)v3);
  if ( !v15 )
    return 1;
  v4 = &v12;
  LOBYTE(v4) = v15 != &v12;
  ((void (__fastcall *)(void ***, void ***))(*v15)[4])(v15, v4);
  return 1;
}


==========

FUNCTION: ?Add@ValueList@cxl@@QEAA_NAEBV?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@TCLUSPROP_SYNTAX@@@Z @ 0x180016684
----------
bool __fastcall cxl::ValueList::Add(struct cxl::ValueList *this, __int64 a2, union CLUSPROP_SYNTAX a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  LODWORD(v22) = a3;
  if ( a3.wFormat != 5 )
  {
    v18 = std::wstring::wstring(&v23, L"The syntax is not valid for a CLUSPROP_MULTI_SZ value");
    v22 = 87i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v22, (__int64)v18);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  *(_OWORD *)v20 = 0i64;
  v5 = 0i64;
  v21 = 0i64;
  v6 = 0i64;
  v7 = *(_QWORD *)a2;
  v8 = 0i64;
  v9 = 0i64;
  while ( v7 != *(_QWORD *)(a2 + 8) )
  {
    v10 = (v8 - v9) /*signed*/>> 1;
    v11 = *(_QWORD *)(v7 + 16) + v10 + 1;
    if ( v11 >= v10 )
    {
      if ( v11 <= v10 )
        goto LABEL_11;
      if ( v11 > (v5 - (__int64)v9) /*signed*/>> 1 )
      {
        std::vector<wchar_t>::_Resize_reallocate<std::_Value_init_tag>((const void **)v20, *(_QWORD *)(v7 + 16) + v10 + 1, (__int64)v9);
        v8 = v20[1];
        goto LABEL_11;
      }
      v12 = 2 * (*(_QWORD *)(v7 + 16) + 1i64);
      memset_0(v8, 0, v12);
      v8 += v12;
    }
    else
    {
      v8 = &v9[2 * v11];
    }
    v20[1] = v8;
LABEL_11:
    v13 = std::_String_val<std::_Simple_types<wchar_t>>::_Myptr(v7);
    _o_wcscpy_s(v14 + 2 * v6, ((__int64)&v8[-v14] /*signed*/>> 1) - v6, v13);
    v8 = v20[1];
    v9 = v20[0];
    v6 = (v20[1] - v20[0]) /*signed*/>> 1;
    v7 += 32i64;
    v5 = v21;
  }
  v19 = 0;
  std::vector<wchar_t>::push_back((__int64)v20, &v19);
  *(_QWORD *)&v23 = v20;
  WORD4(v23) = 5;
  WORD5(v23) = WORD1(v22);
  v24 = &std::_Func_impl_no_alloc<_lambda_fc83ba0a093fc81c78ee16260397cfad_,void,CLUSPROP_SZ *>::`vftable';
  v25 = v23;
  v26 = &v24;
  v15 = cxl::ValueList::alloc(this, (2 * ((v20[1] - v20[0]) /*signed*/>> 1) + 11) & 0xFFFFFFFFFFFFFFFCui64);
  std::_Func_class<void,CLUSPROP_SZ *>::operator()((__int64)&v24, (__int64)v15);
  if ( v26 )
  {
    v16 = &v24;
    LOBYTE(v16) = v26 != &v24;
    ((void (__fastcall *)(void ***, void ***))(*v26)[4])(v26, v16);
    v26 = 0i64;
  }
  std::vector<wchar_t>::_Tidy((__int64)v20);
  return 1;
}


==========

FUNCTION: ?Add@ValueList@cxl@@QEAA_NGTCLUSPROP_SYNTAX@@@Z @ 0x1800168BC
----------
bool __fastcall cxl::ValueList::Add(struct cxl::ValueList *this, unsigned __int16 a2, union CLUSPROP_SYNTAX a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v13 = a2;
  if ( a3.wFormat != 11 )
  {
    v6 = std::wstring::wstring(v11, L"The syntax is not valid for a CLUSPROP_WORD value");
    *(_QWORD *)&v7 = 87i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v7, (__int64)v6);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  *(_QWORD *)&v7 = &v13;
  WORD4(v7) = 11;
  WORD5(v7) = a3.wType;
  v8 = &std::_Func_impl_no_alloc<_lambda_816208a69cc4b0b4c355bcbf77f68f55_,void,CLUSPROP_WORD *>::`vftable';
  v9 = v7;
  v10 = &v8;
  *(_QWORD *)&v7 = cxl::ValueList::alloc(this, 0xCui64);
  v3 = (__int64)v10;
  if ( !v10 )
  {
    std::_Xbad_function_call();
    __debugbreak();
  }
  (*(void (__fastcall **)(__int64, __int128 *))(*(_QWORD *)v3 + 16i64))(v3, &v7);
  if ( !v10 )
    return 1;
  v4 = &v8;
  LOBYTE(v4) = v10 != &v8;
  ((void (__fastcall *)(void ***, void ***))(*v10)[4])(v10, v4);
  return 1;
}


==========

FUNCTION: ?Add@ValueList@cxl@@QEAA_NJTCLUSPROP_SYNTAX@@@Z @ 0x1800169DC
----------
bool __fastcall cxl::ValueList::Add(struct cxl::ValueList *this, int a2, union CLUSPROP_SYNTAX a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v13 = a2;
  if ( a3.wFormat != 7 )
  {
    v6 = std::wstring::wstring(v11, L"The syntax is not valid for a CLUSPROP_LONG value");
    *(_QWORD *)&v7 = 87i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v7, (__int64)v6);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  *(_QWORD *)&v7 = &v13;
  WORD4(v7) = 7;
  WORD5(v7) = a3.wType;
  v8 = &std::_Func_impl_no_alloc<_lambda_c7cdd02fd239d044c409948cce4e8bb3_,void,CLUSPROP_LONG *>::`vftable';
  v9 = v7;
  v10 = &v8;
  *(_QWORD *)&v7 = cxl::ValueList::alloc(this, 0xCui64);
  v3 = (__int64)v10;
  if ( !v10 )
  {
    std::_Xbad_function_call();
    __debugbreak();
  }
  (*(void (__fastcall **)(__int64, __int128 *))(*(_QWORD *)v3 + 16i64))(v3, &v7);
  if ( !v10 )
    return 1;
  v4 = &v8;
  LOBYTE(v4) = v10 != &v8;
  ((void (__fastcall *)(void ***, void ***))(*v10)[4])(v10, v4);
  return 1;
}


==========

FUNCTION: ?Add@ValueList@cxl@@QEAA_NKTCLUSPROP_SYNTAX@@@Z @ 0x180016AFC
----------
bool __fastcall cxl::ValueList::Add(struct cxl::ValueList *this, unsigned int a2, union CLUSPROP_SYNTAX a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v13 = a2;
  if ( a3.wFormat != 2 )
  {
    v6 = std::wstring::wstring(v11, L"The syntax is not valid for a CLUSPROP_DWORD value");
    *(_QWORD *)&v7 = 87i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v7, (__int64)v6);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  *(_QWORD *)&v7 = &v13;
  WORD4(v7) = 2;
  WORD5(v7) = a3.wType;
  v8 = &std::_Func_impl_no_alloc<_lambda_0b67125c53526fe26e42f7c3c0b17e56_,void,CLUSPROP_DWORD *>::`vftable';
  v9 = v7;
  v10 = &v8;
  *(_QWORD *)&v7 = cxl::ValueList::alloc(this, 0xCui64);
  v3 = (__int64)v10;
  if ( !v10 )
  {
    std::_Xbad_function_call();
    __debugbreak();
  }
  (*(void (__fastcall **)(__int64, __int128 *))(*(_QWORD *)v3 + 16i64))(v3, &v7);
  if ( !v10 )
    return 1;
  v4 = &v8;
  LOBYTE(v4) = v10 != &v8;
  ((void (__fastcall *)(void ***, void ***))(*v10)[4])(v10, v4);
  return 1;
}


==========

FUNCTION: ?Add@ValueList@cxl@@QEAA_NPEBE_KTCLUSPROP_SYNTAX@@@Z @ 0x180016C1C
----------
bool __fastcall cxl::ValueList::Add(struct cxl::ValueList *this, const unsigned __int8 *a2, unsigned __int64 a3, union CLUSPROP_SYNTAX a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)&v8 = a2;
  DWORD2(v8) = a4;
  v9 = a3;
  v10 = &std::_Func_impl_no_alloc<_lambda_e7bb0fc19b685c49413d07135726e895_,void,CLUSPROP_BINARY *>::`vftable';
  v11 = v8;
  v12 = a3;
  v13 = &v10;
  v7 = cxl::ValueList::alloc(this, (a3 + 11) & 0xFFFFFFFFFFFFFFFCui64);
  v4 = (__int64)v13;
  if ( !v13 )
  {
    std::_Xbad_function_call();
    __debugbreak();
  }
  (*(void (__fastcall **)(__int64, struct CLUSPROP_VALUE **))(*(_QWORD *)v4 + 16i64))(v4, &v7);
  if ( !v13 )
    return 1;
  v5 = &v10;
  LOBYTE(v5) = v13 != &v10;
  ((void (__fastcall *)(void ***, void ***))(*v13)[4])(v13, v5);
  return 1;
}


==========

FUNCTION: ?Add@ValueList@cxl@@QEAA_NT_LARGE_INTEGER@@TCLUSPROP_SYNTAX@@@Z @ 0x180016CE0
----------
bool __fastcall cxl::ValueList::Add(struct cxl::ValueList *this, union _LARGE_INTEGER a2, union CLUSPROP_SYNTAX a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v8 = a2;
  if ( a3.wFormat != 10 )
  {
    v6 = std::wstring::wstring(v12, L"The syntax is not valid for a CLUSPROP_LARGE_INTEGER value");
    *(_QWORD *)&v7 = 87i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v7, (__int64)v6);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  *(_QWORD *)&v7 = &v8;
  WORD4(v7) = 10;
  WORD5(v7) = a3.wType;
  v9 = &std::_Func_impl_no_alloc<_lambda_e2961530ac116b645cf2739c01b935c9_,void,CLUSPROP_LARGE_INTEGER *>::`vftable';
  v10 = v7;
  v11 = &v9;
  *(_QWORD *)&v7 = cxl::ValueList::alloc(this, 0x10ui64);
  v3 = (__int64)v11;
  if ( !v11 )
  {
    std::_Xbad_function_call();
    __debugbreak();
  }
  (*(void (__fastcall **)(__int64, __int128 *))(*(_QWORD *)v3 + 16i64))(v3, &v7);
  if ( !v11 )
    return 1;
  v4 = &v9;
  LOBYTE(v4) = v11 != &v9;
  ((void (__fastcall *)(void ***, void ***))(*v11)[4])(v11, v4);
  return 1;
}


==========

FUNCTION: ?Add@ValueList@cxl@@QEAA_NT_ULARGE_INTEGER@@TCLUSPROP_SYNTAX@@@Z @ 0x180016E04
----------
bool __fastcall cxl::ValueList::Add(struct cxl::ValueList *this, union _ULARGE_INTEGER a2, union CLUSPROP_SYNTAX a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v8 = a2;
  if ( a3.wFormat != 6 )
  {
    v6 = std::wstring::wstring(v12, L"The syntax is not valid for a CLUSPROP_ULARGE_INTEGER value");
    *(_QWORD *)&v7 = 87i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v7, (__int64)v6);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  *(_QWORD *)&v7 = &v8;
  WORD4(v7) = 6;
  WORD5(v7) = a3.wType;
  v9 = &std::_Func_impl_no_alloc<_lambda_c7c822279bd82bb36f00a0e1e4f6b2c0_,void,CLUSPROP_ULARGE_INTEGER *>::`vftable';
  v10 = v7;
  v11 = &v9;
  *(_QWORD *)&v7 = cxl::ValueList::alloc(this, 0x10ui64);
  v3 = (__int64)v11;
  if ( !v11 )
  {
    std::_Xbad_function_call();
    __debugbreak();
  }
  (*(void (__fastcall **)(__int64, __int128 *))(*(_QWORD *)v3 + 16i64))(v3, &v7);
  if ( !v11 )
    return 1;
  v4 = &v9;
  LOBYTE(v4) = v11 != &v9;
  ((void (__fastcall *)(void ***, void ***))(*v11)[4])(v11, v4);
  return 1;
}


==========

FUNCTION: ?Add@ValueList@cxl@@QEAA_NU_FILETIME@@TCLUSPROP_SYNTAX@@@Z @ 0x180016F28
----------
bool __fastcall cxl::ValueList::Add(struct cxl::ValueList *this, struct _FILETIME a2, union CLUSPROP_SYNTAX a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v8 = a2;
  if ( a3.wFormat != 12 )
  {
    v6 = std::wstring::wstring(v12, L"The syntax is not valid for a CLUSPROP_FILETIME value");
    *(_QWORD *)&v7 = 87i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v7, (__int64)v6);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  *(_QWORD *)&v7 = &v8;
  WORD4(v7) = 12;
  WORD5(v7) = a3.wType;
  v9 = &std::_Func_impl_no_alloc<_lambda_6f966ccf4514af3f526a1f1ead854bbb_,void,CLUSPROP_FILETIME *>::`vftable';
  v10 = v7;
  v11 = &v9;
  *(_QWORD *)&v7 = cxl::ValueList::alloc(this, 0x10ui64);
  v3 = (__int64)v11;
  if ( !v11 )
  {
    std::_Xbad_function_call();
    __debugbreak();
  }
  (*(void (__fastcall **)(__int64, __int128 *))(*(_QWORD *)v3 + 16i64))(v3, &v7);
  if ( !v11 )
    return 1;
  v4 = &v9;
  LOBYTE(v4) = v11 != &v9;
  ((void (__fastcall *)(void ***, void ***))(*v11)[4])(v11, v4);
  return 1;
}


==========

FUNCTION: ?ParseBuffer@ValueList@cxl@@AEAA_KQEBE_K@Z @ 0x180017080
----------
unsigned __int64 __fastcall cxl::ValueList::ParseBuffer(struct cxl::ValueList *this, const unsigned __int8 *const a2, unsigned __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a3 )
  {
    if ( !a2 )
    {
      v4 = (_BYTE *)this->field_8;
      v5 = this->vector_00;
      v6 = v4 - (_BYTE *)this->vector_00;
      if ( v6 > 4 )
      {
        v7 = (__int64)v5 + 4;
LABEL_9:
        this->field_8 = v7;
        goto LABEL_10;
      }
      if ( v6 < 4 )
      {
        if ( this->field_10 - (__int64)v5 >= 4ui64 )
        {
          v8 = 4 - v6;
          memset_0(v4, 0, 4 - v6);
          v7 = (__int64)&v4[v8];
          goto LABEL_9;
        }
        std::vector<unsigned char>::_Resize_reallocate<std::_Value_init_tag>((const void **)&this->vector_00, 4ui64, 0i64);
      }
LABEL_10:
      *(_DWORD *)this->vector_00 = 0;
      return this->field_8 - (unsigned __int64)this->vector_00;
    }
LABEL_27:
    v17 = std::wstring::wstring(v19, L"The supplied buffer is not large enough for a value list");
    v18.value_00 = 234;
    v18.code_type_04 = 0;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, &v18, (__int64)v17);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  if ( a3 < 4 )
    goto LABEL_27;
  v10 = 0i64;
  while ( 1 )
  {
    result = v10 + 4;
    v11 = v10 + 4 == a3;
    v12 = v10 + 4 <= a3;
    if ( v10 + 4 >= a3 )
      break;
    if ( !a2 || !*(_DWORD *)a2 )
    {
      v11 = result == a3;
      v12 = result <= a3;
      break;
    }
    v13 = (*((_DWORD *)a2 + 1) + 3) & 0xFFFFFFFC;
    v10 += v13 + 8;
    if ( v10 > a3 )
    {
      v14 = std::wstring::wstring(v19, L"The value list is larger than the provided buffer");
      v18.value_00 = 234;
      v18.code_type_04 = 0;
      cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, &v18, (__int64)v14);
      CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
    }
    a2 += v13 + 8;
  }
  if ( !v12 )
  {
    v16 = std::wstring::wstring(v19, L"The supplied buffer is not large enough for the final CLUSPROP_SYNTAX_ENDMARK syntax");
    v18.value_00 = 234;
    v18.code_type_04 = 0;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, &v18, (__int64)v16);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  if ( !v11 || !a2 || !*(_DWORD *)a2 )
    return result;
  v15 = std::wstring::wstring(v19, L"The supplied buffer does not contain the final CLUSPROP_SYNTAX_ENDMARK syntax");
  v18.value_00 = 234;
  v18.code_type_04 = 0;
  cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, &v18, (__int64)v15);
  CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  return result;
}


==========

FUNCTION: ?alloc@ValueList@cxl@@AEAAPEAUCLUSPROP_VALUE@@_K@Z @ 0x180017D80
----------
struct CLUSPROP_VALUE *__fastcall cxl::ValueList::alloc(struct cxl::ValueList *this, unsigned __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( this->field_50 )
    return (struct CLUSPROP_VALUE *)std::_Func_class<CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::operator()((__int64)&this->end_mark_18, 4i64, (int)a2);
  v5 = (_BYTE *)(this->field_8 - 4);
  v6 = 0;
  std::vector<unsigned char>::insert(&this->vector_00, &v7, v5, a2, &v6);
  return (struct CLUSPROP_VALUE *)(this->field_8 - a2 - 4);
}


==========

FUNCTION: ?begin@ValueList@cxl@@QEAA?AVvalue_iterator@12@XZ @ 0x180017DEC
----------
struct cxl::ValueList::value_iterator *__fastcall cxl::ValueList::begin(__int64 a1, struct cxl::ValueList::value_iterator *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4.buffer_ptr_00 = (void *)a1;
  v4.list_ptr_08 = 0i64;
  v4.flag_10 = 0;
  cxl::ValueList::value_iterator::value_iterator(a2, 0i64, &v4);
  return a2;
}


==========

FUNCTION: ?end@ValueList@cxl@@QEAA?AVvalue_iterator@12@XZ @ 0x180017EC8
----------
struct cxl::ValueList::value_iterator *__fastcall cxl::ValueList::end(__int64 a1, struct cxl::ValueList::value_iterator *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4.buffer_ptr_00 = (void *)a1;
  v4.list_ptr_08 = 0i64;
  v4.flag_10 = 0;
  cxl::ValueList::value_iterator::value_iterator(a2, 0xFFFFFFFFFFFFFFFFui64, &v4);
  return a2;
}


==========

FUNCTION: ?get@list_accessor@ValueList@cxl@@QEBAPEAUCLUSPROP_VALUE@@_K@Z @ 0x180017FEC
----------
struct CLUSPROP_VALUE *__fastcall cxl::ValueList::list_accessor::get(struct cxl::ValueList::list_accessor *this, unsigned __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = this->list_ptr_08;
  if ( v3 )
  {
    v4 = *((_QWORD *)v3 + 2);
    if ( !this->flag_10 )
      return (struct CLUSPROP_VALUE *)(a2 + v4);
    v5 = *((_QWORD *)v3 + 3) - v4;
    result = (struct CLUSPROP_VALUE *)(a2 + v4);
    v7 = v5 < a2 + 8;
  }
  else
  {
    v7 = *((_QWORD *)this->buffer_ptr_00 + 1) - *(_QWORD *)this->buffer_ptr_00 < a2 + 8;
    result = (struct CLUSPROP_VALUE *)(*(_QWORD *)this->buffer_ptr_00 + a2);
  }
  if ( v7 )
    result = 0i64;
  return result;
}


==========

FUNCTION: ?getValue@?$ValueElement@GUCLUSPROP_WORD@@@ValueList@cxl@@AEBAXPEAG@Z @ 0x180018044
----------
void __fastcall cxl::ValueList::ValueElement<unsigned short,CLUSPROP_WORD>::getValue(__int64 this, unsigned __int16 *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(_QWORD *)(this + 40);
  if ( *(_DWORD *)(v2 + 4) != 2 )
  {
    v3 = std::wstring::wstring(v5, L"The data is not the size of a WORD");
    v4.value_00 = 13;
    v4.code_type_04 = 0;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, &v4, (__int64)v3);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  *a2 = *(_WORD *)(v2 + 8);
}


==========

FUNCTION: ?getValue@?$ValueElement@JUCLUSPROP_LONG@@@ValueList@cxl@@AEBAXPEAJ@Z @ 0x1800180D0
----------
void __fastcall cxl::ValueList::ValueElement<long,CLUSPROP_LONG>::getValue(__int64 this, int *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(_QWORD *)(this + 40);
  if ( *(_DWORD *)(v2 + 4) != 4 )
  {
    v3 = std::wstring::wstring(v5, L"The data is not the size of a LONG");
    v4.value_00 = 13;
    v4.code_type_04 = 0;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, &v4, (__int64)v3);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  *a2 = *(_DWORD *)(v2 + 8);
}


==========

FUNCTION: ?getValue@?$ValueElement@KUCLUSPROP_DWORD@@@ValueList@cxl@@AEBAXPEAK@Z @ 0x18001815C
----------
void __fastcall cxl::ValueList::ValueElement<unsigned long,CLUSPROP_DWORD>::getValue(__int64 this, unsigned int *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(_QWORD *)(this + 40);
  if ( *(_DWORD *)(v2 + 4) != 4 )
  {
    v3 = std::wstring::wstring(v5, L"The data is not the size of a DWORD");
    v4.value_00 = 13;
    v4.code_type_04 = 0;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, &v4, (__int64)v3);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  *a2 = *(_DWORD *)(v2 + 8);
}


==========

FUNCTION: ?getValue@?$ValueElement@T_LARGE_INTEGER@@UCLUSPROP_LARGE_INTEGER@@@ValueList@cxl@@AEBAXPEAT_LARGE_INTEGER@@@Z @ 0x1800181E8
----------
void __fastcall cxl::ValueList::ValueElement<_LARGE_INTEGER,CLUSPROP_LARGE_INTEGER>::getValue(__int64 this, union _LARGE_INTEGER *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(union _LARGE_INTEGER **)(this + 40);
  if ( v2->HighPart != 8 )
  {
    v3 = std::wstring::wstring(v5, L"The data is not the size of a LARGE_INTEGER");
    v4.value_00 = 13;
    v4.code_type_04 = 0;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, &v4, (__int64)v3);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  *a2 = v2[1];
}


==========

FUNCTION: ?getValue@?$ValueElement@T_ULARGE_INTEGER@@UCLUSPROP_ULARGE_INTEGER@@@ValueList@cxl@@AEBAXPEAT_ULARGE_INTEGER@@@Z @ 0x180018274
----------
void __fastcall cxl::ValueList::ValueElement<_ULARGE_INTEGER,CLUSPROP_ULARGE_INTEGER>::getValue(__int64 this, union _ULARGE_INTEGER *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(union _ULARGE_INTEGER **)(this + 40);
  if ( v2->HighPart != 8 )
  {
    v3 = std::wstring::wstring(v5, L"The data is not the size of a ULARGE_INTEGER");
    v4.value_00 = 13;
    v4.code_type_04 = 0;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, &v4, (__int64)v3);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  *a2 = v2[1];
}


==========

FUNCTION: ?getValue@?$ValueElement@U_FILETIME@@UCLUSPROP_FILETIME@@@ValueList@cxl@@AEBAXPEAU_FILETIME@@@Z @ 0x180018300
----------
void __fastcall cxl::ValueList::ValueElement<_FILETIME,CLUSPROP_FILETIME>::getValue(__int64 this, struct _FILETIME *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(struct _FILETIME **)(this + 40);
  if ( v2->dwHighDateTime != 8 )
  {
    v3 = std::wstring::wstring(v5, L"The data is not the size of a FILETIME Value");
    v4.value_00 = 13;
    v4.code_type_04 = 0;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, &v4, (__int64)v3);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  *a2 = v2[1];
}


==========

FUNCTION: ?getValue@?$ValueElement@V?$vector@EV?$allocator@E@std@@@std@@UCLUSPROP_BINARY@@@ValueList@cxl@@AEBAXPEAV?$vector@EV?$allocator@E@std@@@std@@@Z @ 0x18001838C
----------
void __fastcall cxl::ValueList::ValueElement<std::vector<unsigned char>,CLUSPROP_BINARY>::getValue(__int64 a1, _QWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(_QWORD *)(a1 + 40);
  v4 = *(unsigned int *)(v2 + 4);
  *(_OWORD *)v8 = 0i64;
  v9 = 0i64;
  v5 = (const void *)(v2 + 8);
  v6 = v4;
  if ( v4 )
  {
    std::vector<unsigned char>::_Buy_nonzero(v8, v4);
    v7 = (char *)v8[0];
    memmove_0(v8[0], v5, v6);
    v10 = 0i64;
    v8[1] = &v7[v6];
    std::_Tidy_guard<std::vector<unsigned char>>::~_Tidy_guard<std::vector<unsigned char>>(&v10);
  }
  std::vector<unsigned char>::operator=(a2, v8);
  std::vector<unsigned char>::_Tidy((__int64)v8);
}


==========

FUNCTION: ?getValue@?$ValueElement@V?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@UCLUSPROP_SZ@@@ValueList@cxl@@AEBAXPEAV?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@@Z @ 0x180018434
----------
void __fastcall cxl::ValueList::ValueElement<std::vector<std::wstring>,CLUSPROP_SZ>::getValue(__int64 a1, _QWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = cxl::MultiSz<std::vector<std::wstring>>::MultiSz<std::vector<std::wstring>>(v6, (_WORD *)(*(_QWORD *)(a1 + 40) + 8i64), *(unsigned int *)(*(_QWORD *)(a1 + 40) + 4i64));
  v4 = cxl::MultiSz<std::vector<std::wstring>>::operator std::vector<std::wstring>(v3, v5);
  if ( a2 != v4 )
  {
    std::vector<std::wstring>::_Tidy((__int64)a2);
    *a2 = *v4;
    a2[1] = v4[1];
    a2[2] = v4[2];
    *v4 = 0i64;
    v4[1] = 0i64;
    v4[2] = 0i64;
  }
  std::vector<std::wstring>::_Tidy((__int64)v5);
  std::vector<std::wstring>::_Tidy((__int64)v6);
}


==========

FUNCTION: ?get_ptr@?$ValueItem@U_FILETIME@@@ValueList@cxl@@UEBAQEAUCLUSPROP_VALUE@@XZ @ 0x180018590
----------
__int64 __fastcall cxl::ValueList::ValueItem<_FILETIME>::get_ptr(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return *(_QWORD *)(a1 + 8);
}


==========

FUNCTION: ?get_type@?$ValueItem@U_FILETIME@@@ValueList@cxl@@UEBA?BTCLUSPROP_SYNTAX@@XZ @ 0x1800185A0
----------
_DWORD *__fastcall cxl::ValueList::ValueItem<_FILETIME>::get_type(__int64 a1, _DWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  result = a2;
  *a2 = **(_DWORD **)(a1 + 8);
  return result;
}


==========

FUNCTION: ?get_type@ValueData@cxl@@QEBA?BTCLUSPROP_SYNTAX@@XZ @ 0x1800185B4
----------
union CLUSPROP_SYNTAX __fastcall cxl::ValueData::get_type(struct cxl::ValueData *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2.dw = v1.dw;
  (**(void (__fastcall ***)(void *))this->shared_ptr_00)(this->shared_ptr_00);
  return v2;
}


==========

FUNCTION: ?get_value@?$ValueElement@GUCLUSPROP_WORD@@@ValueList@cxl@@UEBA?BGXZ @ 0x1800185E0
----------
unsigned __int16 __fastcall cxl::ValueList::ValueElement<unsigned short,CLUSPROP_WORD>::get_value(__int64 this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = 0;
  cxl::ValueList::ValueElement<unsigned short,CLUSPROP_WORD>::getValue(this, &v2);
  return v2;
}


==========

FUNCTION: ?get_value@?$ValueElement@JUCLUSPROP_LONG@@@ValueList@cxl@@UEBA?BJXZ @ 0x180018610
----------
int __fastcall cxl::ValueList::ValueElement<long,CLUSPROP_LONG>::get_value(__int64 this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = 0;
  cxl::ValueList::ValueElement<long,CLUSPROP_LONG>::getValue(this, &v2);
  return v2;
}


==========

FUNCTION: ?get_value@?$ValueElement@KUCLUSPROP_DWORD@@@ValueList@cxl@@UEBA?BKXZ @ 0x180018640
----------
unsigned int __fastcall cxl::ValueList::ValueElement<unsigned long,CLUSPROP_DWORD>::get_value(__int64 this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = 0;
  cxl::ValueList::ValueElement<unsigned long,CLUSPROP_DWORD>::getValue(this, &v2);
  return v2;
}


==========

FUNCTION: ?get_value@?$ValueElement@T_LARGE_INTEGER@@UCLUSPROP_LARGE_INTEGER@@@ValueList@cxl@@UEBA?BT_LARGE_INTEGER@@XZ @ 0x180018670
----------
union _LARGE_INTEGER __fastcall cxl::ValueList::ValueElement<_LARGE_INTEGER,CLUSPROP_LARGE_INTEGER>::get_value(__int64 this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2.QuadPart = (LONGLONG)v1;
  v1->QuadPart = 0i64;
  cxl::ValueList::ValueElement<_LARGE_INTEGER,CLUSPROP_LARGE_INTEGER>::getValue(this, v1);
  return v2;
}


==========

FUNCTION: ?get_value@?$ValueElement@T_ULARGE_INTEGER@@UCLUSPROP_ULARGE_INTEGER@@@ValueList@cxl@@UEBA?BT_ULARGE_INTEGER@@XZ @ 0x1800186A0
----------
union _ULARGE_INTEGER __fastcall cxl::ValueList::ValueElement<_ULARGE_INTEGER,CLUSPROP_ULARGE_INTEGER>::get_value(__int64 this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2.QuadPart = (ULONGLONG)v1;
  v1->QuadPart = 0i64;
  cxl::ValueList::ValueElement<_ULARGE_INTEGER,CLUSPROP_ULARGE_INTEGER>::getValue(this, v1);
  return v2;
}


==========

FUNCTION: ?get_value@?$ValueElement@U_FILETIME@@UCLUSPROP_FILETIME@@@ValueList@cxl@@UEBA?BU_FILETIME@@XZ @ 0x1800186D0
----------
struct _FILETIME __fastcall cxl::ValueList::ValueElement<_FILETIME,CLUSPROP_FILETIME>::get_value(__int64 this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = (struct _FILETIME)v1;
  *v1 = 0i64;
  cxl::ValueList::ValueElement<_FILETIME,CLUSPROP_FILETIME>::getValue(this, v1);
  return v2;
}


==========

FUNCTION: ?get_value@?$ValueElement@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UCLUSPROP_SZ@@@ValueList@cxl@@UEBA?BV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@XZ @ 0x180018700
----------
__int64 __fastcall cxl::ValueList::ValueElement<std::wstring,CLUSPROP_SZ>::get_value(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_OWORD *)a2 = 0i64;
  *(_QWORD *)(a2 + 16) = 0i64;
  *(_QWORD *)(a2 + 24) = 7i64;
  *(_WORD *)a2 = 0;
  v3 = (_WORD *)(*(_QWORD *)(a1 + 40) + 8i64);
  v4 = -1i64;
  do
    ++v4;
  while ( v3[v4] );
  std::wstring::assign(a2, v3, v4);
  return a2;
}


==========

FUNCTION: ?get_value@?$ValueElement@V?$vector@EV?$allocator@E@std@@@std@@UCLUSPROP_BINARY@@@ValueList@cxl@@UEBA?BV?$vector@EV?$allocator@E@std@@@std@@XZ @ 0x180018760
----------
_QWORD *__fastcall cxl::ValueList::ValueElement<std::vector<unsigned char>,CLUSPROP_BINARY>::get_value(__int64 a1, _QWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a2 = 0i64;
  a2[1] = 0i64;
  a2[2] = 0i64;
  cxl::ValueList::ValueElement<std::vector<unsigned char>,CLUSPROP_BINARY>::getValue(a1, a2);
  return a2;
}


==========

FUNCTION: ?get_value@?$ValueElement@V?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@UCLUSPROP_SZ@@@ValueList@cxl@@UEBA?BV?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@XZ @ 0x1800187A0
----------
_QWORD *__fastcall cxl::ValueList::ValueElement<std::vector<std::wstring>,CLUSPROP_SZ>::get_value(__int64 a1, _QWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a2 = 0i64;
  a2[1] = 0i64;
  a2[2] = 0i64;
  cxl::ValueList::ValueElement<std::vector<std::wstring>,CLUSPROP_SZ>::getValue(a1, a2);
  return a2;
}


==========

FUNCTION: ?get_value@value_iterator@ValueList@cxl@@AEAAXXZ @ 0x1800187DC
----------
void __fastcall cxl::ValueList::value_iterator::get_value(struct cxl::ValueList::value_iterator *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = (_QWORD *)((char *)this + 24);
  v3 = cxl::ValueList::list_accessor::get((struct cxl::ValueList::list_accessor *)((char *)this + 24), *(_QWORD *)this);
  if ( *(_QWORD *)this == -1i64 || !v3 )
    return;
  v4 = v3->Syntax_00.wFormat;
  if ( v4 > 0x8000 )
    goto LABEL_29;
  if ( v4 == 0x8000 )
    goto LABEL_33;
  if ( v4 > 8 )
  {
    v13 = v4 - 9;
    if ( v13 )
    {
      v14 = v13 - 1;
      if ( !v14 )
      {
        v19 = (__int64)v3;
        v11 = operator new(0x40ui64);
        v20[0] = (__int64)v11;
        *(_OWORD *)v11 = 0i64;
        v11[2] = 1;
        v11[3] = 1;
        *(_QWORD *)v11 = &std::_Ref_count_obj2<cxl::ValueList::ValueElement<_LARGE_INTEGER,CLUSPROP_LARGE_INTEGER>>::`vftable';
        std::_Construct_in_place<cxl::ValueList::ValueElement<_LARGE_INTEGER,CLUSPROP_LARGE_INTEGER>,CLUSPROP_LARGE_INTEGER *,cxl::ValueList::list_accessor &>((_QWORD *)v11 + 2, &v19, v2);
        goto LABEL_18;
      }
      v15 = v14 - 1;
      if ( !v15 )
      {
        v19 = (__int64)v3;
        v11 = operator new(0x40ui64);
        v20[0] = (__int64)v11;
        *(_OWORD *)v11 = 0i64;
        v11[2] = 1;
        v11[3] = 1;
        *(_QWORD *)v11 = &std::_Ref_count_obj2<cxl::ValueList::ValueElement<unsigned short,CLUSPROP_WORD>>::`vftable';
        std::_Construct_in_place<cxl::ValueList::ValueElement<unsigned short,CLUSPROP_WORD>,CLUSPROP_WORD *,cxl::ValueList::list_accessor &>((_QWORD *)v11 + 2, &v19, v2);
        goto LABEL_18;
      }
      v16 = v15 - 1;
      if ( !v16 )
      {
        v19 = (__int64)v3;
        v11 = operator new(0x40ui64);
        v20[0] = (__int64)v11;
        *(_OWORD *)v11 = 0i64;
        v11[2] = 1;
        v11[3] = 1;
        *(_QWORD *)v11 = &std::_Ref_count_obj2<cxl::ValueList::ValueElement<_FILETIME,CLUSPROP_FILETIME>>::`vftable';
        std::_Construct_in_place<cxl::ValueList::ValueElement<_FILETIME,CLUSPROP_FILETIME>,CLUSPROP_FILETIME *,cxl::ValueList::list_accessor &>((_QWORD *)v11 + 2, &v19, v2);
        goto LABEL_18;
      }
      v17 = v16 - 1;
      if ( v17 )
      {
        if ( v17 != 1 )
          goto LABEL_29;
      }
    }
    goto LABEL_33;
  }
  if ( v4 == 8 )
    goto LABEL_22;
  v5 = v4 - 1;
  if ( !v5 )
  {
LABEL_33:
    v19 = (__int64)v3;
    v11 = operator new(0x40ui64);
    v20[0] = (__int64)v11;
    *(_OWORD *)v11 = 0i64;
    v11[2] = 1;
    v11[3] = 1;
    *(_QWORD *)v11 = &std::_Ref_count_obj2<cxl::ValueList::ValueElement<std::vector<unsigned char>,CLUSPROP_BINARY>>::`vftable';
    std::_Construct_in_place<cxl::ValueList::ValueElement<std::vector<unsigned char>,CLUSPROP_BINARY>,CLUSPROP_BINARY *,cxl::ValueList::list_accessor &>((_QWORD *)v11 + 2, &v19, v2);
    goto LABEL_18;
  }
  v6 = v5 - 1;
  if ( !v6 )
  {
    v19 = (__int64)v3;
    v11 = operator new(0x40ui64);
    v20[0] = (__int64)v11;
    *(_OWORD *)v11 = 0i64;
    v11[2] = 1;
    v11[3] = 1;
    *(_QWORD *)v11 = &std::_Ref_count_obj2<cxl::ValueList::ValueElement<unsigned long,CLUSPROP_DWORD>>::`vftable';
    std::_Construct_in_place<cxl::ValueList::ValueElement<unsigned long,CLUSPROP_DWORD>,CLUSPROP_DWORD *,cxl::ValueList::list_accessor &>((_QWORD *)v11 + 2, &v19, v2);
    goto LABEL_18;
  }
  v7 = v6 - 1;
  if ( !v7 || (v8 = v7 - 1) == 0 )
  {
LABEL_22:
    v19 = (__int64)v3;
    v11 = operator new(0x40ui64);
    v20[0] = (__int64)v11;
    *(_OWORD *)v11 = 0i64;
    v11[2] = 1;
    v11[3] = 1;
    *(_QWORD *)v11 = &std::_Ref_count_obj2<cxl::ValueList::ValueElement<std::wstring,CLUSPROP_SZ>>::`vftable';
    std::_Construct_in_place<cxl::ValueList::ValueElement<std::wstring,CLUSPROP_SZ>,CLUSPROP_SZ *,cxl::ValueList::list_accessor &>((_QWORD *)v11 + 2, &v19, v2);
    goto LABEL_18;
  }
  v9 = v8 - 1;
  if ( !v9 )
  {
    v19 = (__int64)v3;
    v11 = operator new(0x40ui64);
    v20[0] = (__int64)v11;
    *(_OWORD *)v11 = 0i64;
    v11[2] = 1;
    v11[3] = 1;
    *(_QWORD *)v11 = &std::_Ref_count_obj2<cxl::ValueList::ValueElement<std::vector<std::wstring>,CLUSPROP_SZ>>::`vftable';
    std::_Construct_in_place<cxl::ValueList::ValueElement<std::vector<std::wstring>,CLUSPROP_SZ>,CLUSPROP_SZ *,cxl::ValueList::list_accessor &>((_QWORD *)v11 + 2, &v19, v2);
    goto LABEL_18;
  }
  v10 = v9 - 1;
  if ( !v10 )
  {
    v19 = (__int64)v3;
    v11 = operator new(0x40ui64);
    v20[0] = (__int64)v11;
    *(_OWORD *)v11 = 0i64;
    v11[2] = 1;
    v11[3] = 1;
    *(_QWORD *)v11 = &std::_Ref_count_obj2<cxl::ValueList::ValueElement<_ULARGE_INTEGER,CLUSPROP_ULARGE_INTEGER>>::`vftable';
    std::_Construct_in_place<cxl::ValueList::ValueElement<_ULARGE_INTEGER,CLUSPROP_ULARGE_INTEGER>,CLUSPROP_ULARGE_INTEGER *,cxl::ValueList::list_accessor &>((_QWORD *)v11 + 2, &v19, v2);
    goto LABEL_18;
  }
  if ( v10 != 1 )
  {
LABEL_29:
    v18 = std::wstring::wstring(v21, L"Invalid syntax");
    v19 = 13i64;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, (const struct cxl::ErrorCode *)&v19, (__int64)v18);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  v19 = (__int64)v3;
  v11 = operator new(0x40ui64);
  v20[0] = (__int64)v11;
  *(_OWORD *)v11 = 0i64;
  v11[2] = 1;
  v11[3] = 1;
  *(_QWORD *)v11 = &std::_Ref_count_obj2<cxl::ValueList::ValueElement<long,CLUSPROP_LONG>>::`vftable';
  std::_Construct_in_place<cxl::ValueList::ValueElement<long,CLUSPROP_LONG>,CLUSPROP_LONG *,cxl::ValueList::list_accessor &>((_QWORD *)v11 + 2, &v19, v2);
LABEL_18:
  v20[0] = v12;
  v20[1] = (__int64)v11;
  cxl::ValueData::ValueData(v21, (__int64)v20);
  std::shared_ptr<mi::MiApplication>::operator=((__int64 *)this + 1);
  if ( v21[0].unknown_08 )
    std::_Ref_count_base::_Decref((std::_Ref_count_base *)v21[0].unknown_08);
  std::_Ref_count_base::_Decref((std::_Ref_count_base *)v11);
}


==========

FUNCTION: ?initialize@value_iterator@ValueList@cxl@@AEAAXXZ @ 0x180018BF4
----------
void __fastcall cxl::ValueList::value_iterator::initialize(struct cxl::ValueList::value_iterator *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( *(_QWORD *)this != -1i64 && ((v2 = (struct cxl::ValueList::list_accessor *)((char *)this + 24), v2->buffer_ptr_00) || v2->list_ptr_08) && (v3 = cxl::ValueList::list_accessor::get(v2, *(_QWORD *)this)) != 0i64 && v3->Syntax_00.dw )
    cxl::ValueList::value_iterator::get_value(this);
  else
    *(_QWORD *)this = -1i64;
}


==========

FUNCTION: ?next_value@value_iterator@ValueList@cxl@@AEAAXXZ @ 0x180019104
----------
void __fastcall cxl::ValueList::value_iterator::next_value(struct cxl::ValueList::value_iterator *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v1 = *(_QWORD *)this;
  if ( *(_QWORD *)this != -1i64 )
  {
    v2 = cxl::ValueList::list_accessor::get((struct cxl::ValueList::list_accessor *)((char *)this + 24), *(_QWORD *)this);
    if ( v2 )
    {
      if ( v2->Syntax_00.dw && (v4 = v1 + ((v2->cbLength_04 + 3) & 0xFFFFFFFC) + 8i64, *(_QWORD *)v3 = v4, (v5 = cxl::ValueList::list_accessor::get((struct cxl::ValueList::list_accessor *)(v3 + 24), v4)) != 0i64) && v5->Syntax_00.dw )
        cxl::ValueList::value_iterator::get_value((struct cxl::ValueList::value_iterator *)v3);
      else
        *(_QWORD *)v3 = -1i64;
    }
  }
}


==========

FUNCTION: ?resize@ValueList@cxl@@AEAAPEAUCLUSPROP_VALUE@@QEAU3@H@Z @ 0x1800192C0
----------
struct CLUSPROP_VALUE *__fastcall cxl::ValueList::resize(struct cxl::ValueList *this, struct CLUSPROP_VALUE *const a2, int a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v5 = (char *)this->vector_00;
  v6 = (char *)a2 - (char *)this->vector_00 + 8;
  v7 = this->field_8 - (unsigned __int64)this->vector_00;
  if ( v6 > v7 - 4 || (v8 = -a3, a3 /*signed*/< 0) && v8 > v7 + 4 )
  {
    v10 = std::wstring::wstring(v13, L"The start address and size must be within the buffer range");
    v12.value_00 = 87;
    v12.code_type_04 = 0;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, &v12, (__int64)v10);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  if ( a3 /*signed*/<= 0 )
  {
    std::vector<unsigned char>::erase((__int64)this, &v12, &v5[v6], &v5[v8 + v6]);
  }
  else
  {
    v11[0] = 0;
    std::vector<unsigned char>::insert(&this->vector_00, &v12, &v5[v6], a3, v11);
  }
  return (struct CLUSPROP_VALUE *)((char *)this->vector_00 + v6 - 8);
}


==========

FUNCTION: ?resize@list_accessor@ValueList@cxl@@QEAAPEAUCLUSPROP_VALUE@@QEAU4@H@Z @ 0x1800193D4
----------
struct CLUSPROP_VALUE *__fastcall cxl::ValueList::list_accessor::resize(struct cxl::ValueList::list_accessor *this, struct CLUSPROP_VALUE *const a2, int a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = (struct cxl::PropertyList *)this->list_ptr_08;
  if ( v4 )
    result = cxl::PropertyList::resize(v4, a2, a3);
  else
    result = cxl::ValueList::resize((struct cxl::ValueList *)this->buffer_ptr_00, a2, a3);
  return result;
}


==========

FUNCTION: ?setValue@?$ValueElement@GUCLUSPROP_WORD@@@ValueList@cxl@@AEAAXAEBG@Z @ 0x1800193F4
----------
void __fastcall cxl::ValueList::ValueElement<unsigned short,CLUSPROP_WORD>::setValue(__int64 this, const unsigned __int16 *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(_QWORD *)(this + 40);
  if ( *(_DWORD *)(v2 + 4) != 2 )
  {
    v3 = std::wstring::wstring(v5, L"The data is not the size of a WORD");
    v4.value_00 = 13;
    v4.code_type_04 = 0;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, &v4, (__int64)v3);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  *(_WORD *)(v2 + 8) = *a2;
}


==========

FUNCTION: ?setValue@?$ValueElement@JUCLUSPROP_LONG@@@ValueList@cxl@@AEAAXAEBJ@Z @ 0x180019484
----------
void __fastcall cxl::ValueList::ValueElement<long,CLUSPROP_LONG>::setValue(__int64 this, const int *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(_QWORD *)(this + 40);
  if ( *(_DWORD *)(v2 + 4) != 4 )
  {
    v3 = std::wstring::wstring(v5, L"The data is not the size of a LONG");
    v4.value_00 = 13;
    v4.code_type_04 = 0;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, &v4, (__int64)v3);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  *(_DWORD *)(v2 + 8) = *a2;
}


==========

FUNCTION: ?setValue@?$ValueElement@KUCLUSPROP_DWORD@@@ValueList@cxl@@AEAAXAEBK@Z @ 0x180019510
----------
void __fastcall cxl::ValueList::ValueElement<unsigned long,CLUSPROP_DWORD>::setValue(__int64 this, const unsigned int *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(_QWORD *)(this + 40);
  if ( *(_DWORD *)(v2 + 4) != 4 )
  {
    v3 = std::wstring::wstring(v5, L"The data is not the size of a DWORD");
    v4.value_00 = 13;
    v4.code_type_04 = 0;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, &v4, (__int64)v3);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  *(_DWORD *)(v2 + 8) = *a2;
}


==========

FUNCTION: ?setValue@?$ValueElement@T_LARGE_INTEGER@@UCLUSPROP_LARGE_INTEGER@@@ValueList@cxl@@AEAAXAEBT_LARGE_INTEGER@@@Z @ 0x18001959C
----------
void __fastcall cxl::ValueList::ValueElement<_LARGE_INTEGER,CLUSPROP_LARGE_INTEGER>::setValue(__int64 this, const union _LARGE_INTEGER *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(union _LARGE_INTEGER **)(this + 40);
  if ( v2->HighPart != 8 )
  {
    v3 = std::wstring::wstring(v5, L"The data is not the size of a LARGE_INTEGER");
    v4.value_00 = 13;
    v4.code_type_04 = 0;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, &v4, (__int64)v3);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  v2[1] = *a2;
}


==========

FUNCTION: ?setValue@?$ValueElement@T_ULARGE_INTEGER@@UCLUSPROP_ULARGE_INTEGER@@@ValueList@cxl@@AEAAXAEBT_ULARGE_INTEGER@@@Z @ 0x18001962C
----------
void __fastcall cxl::ValueList::ValueElement<_ULARGE_INTEGER,CLUSPROP_ULARGE_INTEGER>::setValue(__int64 this, const union _ULARGE_INTEGER *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(union _ULARGE_INTEGER **)(this + 40);
  if ( v2->HighPart != 8 )
  {
    v3 = std::wstring::wstring(v5, L"The data is not the size of a ULARGE_INTEGER");
    v4.value_00 = 13;
    v4.code_type_04 = 0;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, &v4, (__int64)v3);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  v2[1] = *a2;
}


==========

FUNCTION: ?setValue@?$ValueElement@U_FILETIME@@UCLUSPROP_FILETIME@@@ValueList@cxl@@AEAAXAEBU_FILETIME@@@Z @ 0x1800196BC
----------
void __fastcall cxl::ValueList::ValueElement<_FILETIME,CLUSPROP_FILETIME>::setValue(__int64 this, const struct _FILETIME *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(struct _FILETIME **)(this + 40);
  if ( v2->dwHighDateTime != 8 )
  {
    v3 = std::wstring::wstring(v5, L"The data is not the size of a FILETIME Value");
    v4.value_00 = 13;
    v4.code_type_04 = 0;
    cxl::Exception::Exception((struct cxl::Exception *)pExceptionObject, &v4, (__int64)v3);
    CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI2_AVException_cxl__);
  }
  v2[1] = *a2;
}


==========

FUNCTION: ?set_value@?$ValueElement@GUCLUSPROP_WORD@@@ValueList@cxl@@UEAAXAEBG@Z @ 0x180019750
----------
// attributes: thunk
void __fastcall cxl::ValueList::ValueElement<unsigned short,CLUSPROP_WORD>::set_value(__int64 this, const unsigned __int16 *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  cxl::ValueList::ValueElement<unsigned short,CLUSPROP_WORD>::setValue(this, a2);
}


==========

FUNCTION: ?set_value@?$ValueElement@JUCLUSPROP_LONG@@@ValueList@cxl@@UEAAXAEBJ@Z @ 0x180019760
----------
// attributes: thunk
void __fastcall cxl::ValueList::ValueElement<long,CLUSPROP_LONG>::set_value(__int64 this, const int *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  cxl::ValueList::ValueElement<long,CLUSPROP_LONG>::setValue(this, a2);
}


==========

FUNCTION: ?set_value@?$ValueElement@KUCLUSPROP_DWORD@@@ValueList@cxl@@UEAAXAEBK@Z @ 0x180019770
----------
// attributes: thunk
void __fastcall cxl::ValueList::ValueElement<unsigned long,CLUSPROP_DWORD>::set_value(__int64 this, const unsigned int *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  cxl::ValueList::ValueElement<unsigned long,CLUSPROP_DWORD>::setValue(this, a2);
}


==========

FUNCTION: ?set_value@?$ValueElement@T_LARGE_INTEGER@@UCLUSPROP_LARGE_INTEGER@@@ValueList@cxl@@UEAAXAEBT_LARGE_INTEGER@@@Z @ 0x180019780
----------
// attributes: thunk
void __fastcall cxl::ValueList::ValueElement<_LARGE_INTEGER,CLUSPROP_LARGE_INTEGER>::set_value(__int64 this, const union _LARGE_INTEGER *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  cxl::ValueList::ValueElement<_LARGE_INTEGER,CLUSPROP_LARGE_INTEGER>::setValue(this, a2);
}


==========

FUNCTION: ?set_value@?$ValueElement@T_ULARGE_INTEGER@@UCLUSPROP_ULARGE_INTEGER@@@ValueList@cxl@@UEAAXAEBT_ULARGE_INTEGER@@@Z @ 0x180019790
----------
// attributes: thunk
void __fastcall cxl::ValueList::ValueElement<_ULARGE_INTEGER,CLUSPROP_ULARGE_INTEGER>::set_value(__int64 this, const union _ULARGE_INTEGER *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  cxl::ValueList::ValueElement<_ULARGE_INTEGER,CLUSPROP_ULARGE_INTEGER>::setValue(this, a2);
}


==========

FUNCTION: ?set_value@?$ValueElement@U_FILETIME@@UCLUSPROP_FILETIME@@@ValueList@cxl@@UEAAXAEBU_FILETIME@@@Z @ 0x1800197A0
----------
// attributes: thunk
void __fastcall cxl::ValueList::ValueElement<_FILETIME,CLUSPROP_FILETIME>::set_value(__int64 this, const struct _FILETIME *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  cxl::ValueList::ValueElement<_FILETIME,CLUSPROP_FILETIME>::setValue(this, a2);
}


==========

FUNCTION: ?set_value@?$ValueElement@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UCLUSPROP_SZ@@@ValueList@cxl@@UEAAXAEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@Z @ 0x1800197B0
----------
__int64 __fastcall cxl::ValueList::ValueElement<std::wstring,CLUSPROP_SZ>::set_value(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  result = cxl::ValueList::ValueItem<std::wstring>::UpdateValue<std::wstring>(a1, a2, 2i64 * *(_QWORD *)(a2 + 16) + 2);
  *(_QWORD *)(a1 + 40) = result;
  return result;
}


==========

FUNCTION: ?set_value@?$ValueElement@V?$vector@EV?$allocator@E@std@@@std@@UCLUSPROP_BINARY@@@ValueList@cxl@@UEAAXAEBV?$vector@EV?$allocator@E@std@@@std@@@Z @ 0x1800197E0
----------
__int64 __fastcall cxl::ValueList::ValueElement<std::vector<unsigned char>,CLUSPROP_BINARY>::set_value(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  result = cxl::ValueList::ValueItem<std::vector<std::wstring>>::UpdateValue<std::vector<unsigned char>>(a1, (const void *const *)a2, *(_QWORD *)(a2 + 8) - *(_QWORD *)a2);
  *(_QWORD *)(a1 + 40) = result;
  return result;
}


==========

FUNCTION: ?set_value@?$ValueElement@V?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@UCLUSPROP_SZ@@@ValueList@cxl@@UEAAXAEBV?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@@Z @ 0x180019810
----------
__int64 __fastcall cxl::ValueList::ValueElement<std::vector<std::wstring>,CLUSPROP_SZ>::set_value(__int64 a1, __int64 *a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  result = cxl::ValueList::ValueItem<std::vector<std::wstring>>::UpdateValue<std::vector<std::wstring>>(a1, a2, a3);
  *(_QWORD *)(a1 + 40) = result;
  return result;
}


==========

FUNCTION: ?size@?$ValueItem@G@ValueList@cxl@@UEBA?B_KXZ @ 0x180019830
----------
unsigned __int64 __fastcall cxl::ValueList::ValueItem<unsigned short>::size(__int64 this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return *(unsigned int *)(*(_QWORD *)(this + 8) + 4i64);
}


==========

FUNCTION: ?size@ValueData@cxl@@QEBA?B_KXZ @ 0x180019840
----------
unsigned __int64 __fastcall cxl::ValueData::size(struct cxl::ValueData *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return (*(__int64 (__fastcall **)(void *))(*(_QWORD *)this->shared_ptr_00 + 8i64))(this->shared_ptr_00);
}


==========

