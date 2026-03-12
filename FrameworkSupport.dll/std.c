Source file: Microsoft.FailoverClusters.FrameworkSupport
Generated time: 20260130_192837
==========

FUNCTION: ?xsputn@?$basic_streambuf@_WU?$char_traits@_W@std@@@std@@MEAA_JPEB_W_J@Z_0 @ 0x180002560
----------
// attributes: thunk
__int64 std::wstreambuf::xsputn()
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return __imp_?xsputn@?$basic_streambuf@_WU?$char_traits@_W@std@@@std@@MEAA_JPEB_W_J@Z();
}


==========

FUNCTION: ?sync@?$basic_streambuf@_WU?$char_traits@_W@std@@@std@@MEAAHXZ_0 @ 0x180002570
----------
// attributes: thunk
__int64 std::wstreambuf::sync()
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return __imp_?sync@?$basic_streambuf@_WU?$char_traits@_W@std@@@std@@MEAAHXZ();
}


==========

FUNCTION: ?_Unlock@?$basic_streambuf@_WU?$char_traits@_W@std@@@std@@UEAAXXZ_0 @ 0x180002580
----------
// attributes: thunk
__int64 std::wstreambuf::_Unlock()
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return __imp_?_Unlock@?$basic_streambuf@_WU?$char_traits@_W@std@@@std@@UEAAXXZ();
}


==========

FUNCTION: ?_Lock@?$basic_streambuf@_WU?$char_traits@_W@std@@@std@@UEAAXXZ_0 @ 0x180002590
----------
// attributes: thunk
__int64 std::wstreambuf::_Lock()
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return __imp_?_Lock@?$basic_streambuf@_WU?$char_traits@_W@std@@@std@@UEAAXXZ();
}


==========

FUNCTION: ?imbue@?$basic_streambuf@_WU?$char_traits@_W@std@@@std@@MEAAXAEBVlocale@2@@Z_0 @ 0x1800025A0
----------
// attributes: thunk
__int64 std::wstreambuf::imbue()
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return __imp_?imbue@?$basic_streambuf@_WU?$char_traits@_W@std@@@std@@MEAAXAEBVlocale@2@@Z();
}


==========

FUNCTION: ?setbuf@?$basic_streambuf@_WU?$char_traits@_W@std@@@std@@MEAAPEAV12@PEA_W_J@Z_0 @ 0x1800025B0
----------
// attributes: thunk
__int64 std::wstreambuf::setbuf()
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return __imp_?setbuf@?$basic_streambuf@_WU?$char_traits@_W@std@@@std@@MEAAPEAV12@PEA_W_J@Z();
}


==========

FUNCTION: ?xsgetn@?$basic_streambuf@_WU?$char_traits@_W@std@@@std@@MEAA_JPEA_W_J@Z_0 @ 0x1800025C0
----------
// attributes: thunk
__int64 std::wstreambuf::xsgetn()
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return __imp_?xsgetn@?$basic_streambuf@_WU?$char_traits@_W@std@@@std@@MEAA_JPEA_W_J@Z();
}


==========

FUNCTION: ?uflow@?$basic_streambuf@_WU?$char_traits@_W@std@@@std@@MEAAGXZ_0 @ 0x1800025D0
----------
// attributes: thunk
__int64 std::wstreambuf::uflow()
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return __imp_?uflow@?$basic_streambuf@_WU?$char_traits@_W@std@@@std@@MEAAGXZ();
}


==========

FUNCTION: ?showmanyc@?$basic_streambuf@_WU?$char_traits@_W@std@@@std@@MEAA_JXZ_0 @ 0x1800025E0
----------
// attributes: thunk
__int64 std::wstreambuf::showmanyc()
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return __imp_?showmanyc@?$basic_streambuf@_WU?$char_traits@_W@std@@@std@@MEAA_JXZ();
}


==========

FUNCTION: ??0exception@std@@QEAA@AEBV01@@Z @ 0x1800033CC
----------
std::exception *__fastcall std::exception::exception(std::exception *this, const struct std::exception *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)this = &std::exception::`vftable';
  *(_OWORD *)((char *)this + 8) = 0i64;
  o___std_exception_copy_0((char *)a2 + 8);
  return this;
}


==========

FUNCTION: ??1bad_weak_ptr@std@@UEAA@XZ @ 0x180003A3C
----------
void __fastcall std::bad_weak_ptr::~bad_weak_ptr(std::bad_weak_ptr *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)this = &std::exception::`vftable';
  o___std_exception_destroy_0();
}


==========

FUNCTION: ??_Ebad_alloc@std@@UEAAPEAXI@Z @ 0x180003BE0
----------
std::bad_alloc *__fastcall std::bad_alloc::`vector deleting destructor'(std::bad_alloc *this, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)this = &std::exception::`vftable';
  o___std_exception_destroy_0();
  if ( (a2 & 1) != 0 )
    operator delete(this);
  return this;
}


==========

FUNCTION: ?what@exception@std@@UEBAPEBDXZ @ 0x1800085B0
----------
const char *__fastcall std::exception::what(std::exception *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  result = "Unknown exception";
  if ( *((_QWORD *)this + 1) )
    result = (const char *)*((_QWORD *)this + 1);
  return result;
}


==========

FUNCTION: ??$_Allocate@$0BA@U_Default_allocate_traits@std@@$0A@@std@@YAPEAX_K@Z @ 0x18000A8F0
----------
_QWORD *__fastcall std::_Allocate<16,std::_Default_allocate_traits,0>(size_t a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a1 >= 0x1000 )
  {
    if ( a1 + 39 <= a1 )
      std::_Throw_bad_array_new_length();
    v1 = operator new(a1 + 39);
    v2 = v1;
    if ( v1 )
    {
      result = (_QWORD *)(((unsigned __int64)v1 + 39) & 0xFFFFFFFFFFFFFFE0ui64);
      *(result - 1) = v2;
      return result;
    }
    _o__invalid_parameter_noinfo_noreturn();
    __debugbreak();
  }
  if ( a1 )
    result = operator new(a1);
  else
    result = 0i64;
  return result;
}


==========

FUNCTION: ??$_Allocate_at_least_helper@V?$allocator@E@std@@@std@@YAPEAEAEAV?$allocator@E@0@AEA_K@Z @ 0x18000A954
----------
_QWORD *__fastcall std::_Allocate_at_least_helper<std::allocator<unsigned char>>(__int64 a1, size_t *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return std::_Allocate<16,std::_Default_allocate_traits,0>(*a2);
}


==========

FUNCTION: ??$_Allocate_at_least_helper@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@std@@@std@@YAPEAV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@0@AEAV?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@0@AEA_K@Z @ 0x18000A964
----------
_QWORD *__fastcall std::_Allocate_at_least_helper<std::allocator<std::wstring>>(__int64 a1, _QWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = std::_Get_size_of_n<32>(*a2);
  return std::_Allocate<16,std::_Default_allocate_traits,0>(v2);
}


==========

FUNCTION: ??$_Allocate_at_least_helper@V?$allocator@_W@std@@@std@@YAPEA_WAEAV?$allocator@_W@0@AEA_K@Z @ 0x18000A984
----------
_QWORD *__fastcall std::_Allocate_at_least_helper<std::allocator<wchar_t>>(__int64 a1, _QWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( *a2 > 0x7FFFFFFFFFFFFFFFui64 )
    std::_Throw_bad_array_new_length();
  return std::_Allocate<16,std::_Default_allocate_traits,0>(2i64 * *a2);
}


==========

FUNCTION: ??$_Assign_counted_range@PEBE@?$vector@EV?$allocator@E@std@@@std@@AEAAXPEBE_K@Z @ 0x18000A9B4
----------
char *__fastcall std::vector<unsigned char>::_Assign_counted_range<unsigned char const *>(void **a1, char *a2, size_t a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = *a1;
  v4 = a3;
  v5 = a2;
  if ( a3 <= (_BYTE *)a1[2] - (_BYTE *)*a1 )
  {
    v8 = (char *)((_BYTE *)a1[1] - v3);
    if ( a3 > (unsigned __int64)v8 )
    {
      memmove_0(*a1, a2, (_BYTE *)a1[1] - v3);
      v9 = (char *)a1[1];
      v10 = v4 - (_QWORD)v8;
      memmove_0(v9, &v8[(_QWORD)v5], v10);
      result = &v9[v10];
      goto LABEL_4;
    }
  }
  else
  {
    std::vector<unsigned char>::_Clear_and_reserve_geometric(a1, a3);
    v3 = *a1;
    a3 = v4;
    a2 = v5;
  }
  memmove_0(v3, a2, a3);
  result = &v3[v4];
LABEL_4:
  a1[1] = result;
  return result;
}


==========

FUNCTION: ??$_Construct@$00PEB_W@?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEAAXQEB_W_K@Z @ 0x18000AA5C
----------
__int64 __fastcall std::wstring::_Construct<1,wchar_t const *>(_QWORD *a1, const void *a2, unsigned __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a3 > 0x7FFFFFFFFFFFFFFEi64 )
    std::_Xlen_string();
  a1[3] = 7i64;
  if ( a3 > 7 )
  {
    v8 = std::wstring::_Calculate_growth((__int64)a1, a3);
    if ( (unsigned __int64)(v8 + 1) > 0x7FFFFFFFFFFFFFFFi64 )
      std::_Throw_bad_array_new_length();
    v9 = std::_Allocate<16,std::_Default_allocate_traits,0>(2 * (v8 + 1));
    a1[2] = a3;
    v10 = 2 * a3;
    *a1 = v9;
    a1[3] = v8;
    v11 = v9;
    memcpy_0(v9, a2, v10);
    result = 0i64;
    *(_WORD *)((char *)v11 + v10) = 0;
  }
  else
  {
    a1[2] = a3;
    v6 = 2 * a3;
    memcpy_0(a1, a2, 2 * a3);
    result = 0i64;
    *(_WORD *)((char *)a1 + v6) = 0;
  }
  return result;
}


==========

FUNCTION: ??$_Construct@$01PEB_W@?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEAAXQEB_W_K@Z @ 0x18000AB30
----------
__int64 __fastcall std::wstring::_Construct<2,wchar_t const *>(_QWORD *a1, _OWORD *a2, unsigned __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  result = 0x7FFFFFFFFFFFFFFEi64;
  if ( a3 > 0x7FFFFFFFFFFFFFFEi64 )
    std::_Xlen_string();
  a1[3] = 7i64;
  if ( a3 > 7 )
  {
    v7 = std::wstring::_Calculate_growth((__int64)a1, a3);
    if ( (unsigned __int64)(v7 + 1) > 0x7FFFFFFFFFFFFFFFi64 )
      std::_Throw_bad_array_new_length();
    v8 = std::_Allocate<16,std::_Default_allocate_traits,0>(2 * (v7 + 1));
    *a1 = v8;
    a1[2] = a3;
    a1[3] = v7;
    result = (__int64)memcpy_0(v8, a2, 2 * a3 + 2);
  }
  else
  {
    a1[2] = a3;
    *(_OWORD *)a1 = *a2;
  }
  return result;
}


==========

FUNCTION: ??$_Construct_n@AEBQEAV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEBQEAV12@@?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@AEAAX_KAEBQEAV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@1@1@Z @ 0x18000ABE4
----------
void __fastcall std::vector<std::wstring>::_Construct_n<std::wstring * const &,std::wstring * const &>(_QWORD *a1, unsigned __int64 a2, __int64 *a3, __int64 *a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a2 )
  {
    if ( a2 > 0x7FFFFFFFFFFFFFFi64 )
      std::vector<wchar_t>::_Xlength();
    v12 = a2;
    v9 = std::_Allocate_at_least_helper<std::allocator<std::wstring>>((__int64)a1, &v12);
    *a1 = v9;
    a1[1] = v9;
    a1[2] = &v9[4 * a2];
    v12 = (unsigned __int64)a1;
    v10 = *a4;
    for ( i = *a3; i != v10; i += 32i64 )
    {
      std::_Default_allocator_traits<std::allocator<std::wstring>>::construct<std::wstring,std::wstring &>(v8, (__int64)v9, i);
      v9 += 4;
    }
    std::_Destroy_range<std::allocator<std::wstring>>((__int64)v9, (__int64)v9);
    a1[1] = v9;
    v12 = 0i64;
    std::_Tidy_guard<std::vector<std::wstring>>::~_Tidy_guard<std::vector<std::wstring>>(&v12);
  }
}


==========

FUNCTION: ??$_Copy@$0A@@?$_Tree@V?$_Tmap_traits@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@UCaseInsensitive_LessThan@5@V?$allocator@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@@2@$0A@@std@@@std@@IEAAXAEBV01@@Z @ 0x18000ACB8
----------
__int64 __fastcall std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::_Copy<0>(_QWORD *a1, _QWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)(*a1 + 8i64) = std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::_Copy_nodes<0>(a1, *(_QWORD *)(*a2 + 8i64), *a1);
  v4 = (_QWORD *)*a1;
  a1[1] = a2[1];
  v5 = v4[1];
  if ( *(_BYTE *)(v5 + 25) )
  {
    *v4 = v4;
    result = *a1;
    *(_QWORD *)(*a1 + 16i64) = *a1;
  }
  else
  {
    v6 = std::_Tree_val<std::_Tree_simple_types<std::pair<std::wstring const,cxl::PropertyList::Property>>>::_Min((_QWORD *)v5);
    *v8 = v6;
    result = *(_QWORD *)(*a1 + 8i64);
    for ( i = *(_QWORD *)(result + 16); *(_BYTE *)(i + 25) == v7; i = *(_QWORD *)(i + 16) )
      result = i;
    *(_QWORD *)(*a1 + 16i64) = result;
  }
  return result;
}


==========

FUNCTION: ??$_Copy_nodes@$0A@@?$_Tree@V?$_Tmap_traits@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@UCaseInsensitive_LessThan@5@V?$allocator@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@@2@$0A@@std@@@std@@IEAAPEAU?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@1@PEAU21@0@Z @ 0x18000AD3C
----------
_QWORD *__fastcall std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::_Copy_nodes<0>(_QWORD *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v6 = (_QWORD *)*a1;
  if ( *(_BYTE *)(a2 + 25) )
    return v6;
  v10[0] = a1;
  v7 = operator new(0x50ui64);
  std::_Default_allocator_traits<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>::construct<std::pair<std::wstring const,cxl::PropertyList::Property>,std::pair<std::wstring const,cxl::PropertyList::Property> &>(v8, (__int64)(v7 + 4), a2 + 32);
  *v7 = v6;
  v7[1] = v6;
  v7[2] = v6;
  *((_WORD *)v7 + 12) = 0;
  v10[1] = 0i64;
  std::_Alloc_construct_ptr<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>::~_Alloc_construct_ptr<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>((__int64)v10);
  v7[1] = a3;
  *((_BYTE *)v7 + 24) = *(_BYTE *)(a2 + 24);
  if ( *((_BYTE *)v6 + 25) )
    v6 = v7;
  *v7 = std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::_Copy_nodes<0>(a1, *(_QWORD *)a2, (__int64)v7);
  v7[2] = std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::_Copy_nodes<0>(a1, *(_QWORD *)(a2 + 16), (__int64)v7);
  return v6;
}


==========

FUNCTION: ??$_Deallocate@$0BA@$0A@@std@@YAXPEAX_K@Z @ 0x18000AE0C
----------
void __fastcall std::_Deallocate<16,0>(void *a1, unsigned __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = a2;
  Block = a1;
  if ( a2 >= 0x1000 )
  {
    std::_Adjust_manually_vector_aligned(&Block, &v3);
    a1 = Block;
  }
  operator delete(a1);
}


==========

FUNCTION: ??$_Destroy_range@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@std@@@std@@YAXPEAV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@0@QEAV10@AEAV?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@0@@Z @ 0x18000AE4C
----------
void __fastcall std::_Destroy_range<std::allocator<std::wstring>>(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a1 != a2 )
  {
    v3 = a1;
    do
    {
      std::wstring::_Tidy_deallocate(v3);
      v3 += 32i64;
    }
    while ( v3 != a2 );
  }
}


==========

FUNCTION: ??$_Emplace_reallocate@AEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@AEAAPEAV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@1@QEAV21@AEBV21@@Z @ 0x18000AE84
----------
char *__fastcall std::vector<std::wstring>::_Emplace_reallocate<std::wstring const &>(__int64 *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v6 = a2 - *a1;
  v7 = (a1[1] - *a1) /*signed*/>> 5;
  if ( v7 == 0x7FFFFFFFFFFFFFFi64 )
    std::vector<wchar_t>::_Xlength();
  v8 = v7 + 1;
  v17 = std::vector<std::wstring>::_Calculate_growth(a1, v7 + 1);
  v10 = std::_Allocate_at_least_helper<std::allocator<std::wstring>>(v9, &v17);
  v11 = (__int64)v10 + (v6 & 0xFFFFFFFFFFFFFFE0ui64);
  v18 = v11 + 32;
  std::_Default_allocator_traits<std::allocator<std::wstring>>::construct<std::wstring,std::wstring &>(v12, v11, a3);
  v13 = a1[1];
  v14 = (__int64)v10;
  v15 = *a1;
  if ( a2 != v13 )
  {
    std::_Uninitialized_move<std::wstring *>(v15, a2, (__int64)v10);
    v14 = v11 + 32;
    v13 = a1[1];
    v15 = a2;
  }
  std::_Uninitialized_move<std::wstring *>(v15, v13, v14);
  std::vector<std::wstring>::_Change_array((__int64)a1, (__int64)v10, v8, v17);
  return (char *)v11;
}


==========

FUNCTION: ??$_Emplace_reallocate@AEB_W@?$vector@_WV?$allocator@_W@std@@@std@@AEAAPEA_WQEA_WAEB_W@Z @ 0x18000AF6C
----------
_WORD *__fastcall std::vector<wchar_t>::_Emplace_reallocate<wchar_t const &>(const void **a1, _BYTE *a2, _WORD *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v6 = (a2 - (_BYTE *)*a1) /*signed*/>> 1;
  v7 = ((_BYTE *)a1[1] - (_BYTE *)*a1) /*signed*/>> 1;
  if ( v7 == 0x7FFFFFFFFFFFFFFFi64 )
    std::vector<wchar_t>::_Xlength();
  v8 = v7 + 1;
  v9 = std::vector<wchar_t>::_Calculate_growth(a1, v7 + 1);
  v18 = v9;
  v11 = std::_Allocate_at_least_helper<std::allocator<wchar_t>>(v10, &v18);
  v19 = v11;
  v12 = (_WORD *)v11 + v6;
  *v12 = *a3;
  v13 = a1[1];
  v14 = *a1;
  v15 = v11;
  if ( a2 == v13 )
  {
    v16 = v13 - v14;
  }
  else
  {
    memmove_0(v11, v14, a2 - (_BYTE *)*a1);
    v15 = v12 + 1;
    v16 = (_BYTE *)a1[1] - a2;
    v14 = a2;
  }
  memmove_0(v15, v14, v16);
  std::vector<wchar_t>::_Change_array((__int64)a1, (__int64)v11, v8, v9);
  return v12;
}


==========

FUNCTION: ??$_Erase_tree_and_orphan@V?$allocator@U?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@std@@@std@@@?$_Tree_val@U?$_Tree_simple_types@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@@std@@@std@@QEAAXAEAV?$allocator@U?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@std@@@1@PEAU?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@1@@Z @ 0x18000B050
----------
void __fastcall std::_Tree_val<std::_Tree_simple_types<std::pair<std::wstring const,cxl::PropertyList::Property>>>::_Erase_tree_and_orphan<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>(__int64 a1, __int64 a2, __int64 *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = a3;
  while ( !*((_BYTE *)v3 + 25) )
  {
    std::_Tree_val<std::_Tree_simple_types<std::pair<std::wstring const,cxl::PropertyList::Property>>>::_Erase_tree_and_orphan<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>(a1, a2, (__int64 *)v3[2]);
    v6 = (char *)v3;
    v3 = (__int64 *)*v3;
    std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>::_Freenode<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>(v7, v6);
  }
}


==========

FUNCTION: ??$_Freenode@V?$allocator@U?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@std@@@std@@@?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@std@@SAXAEAV?$allocator@U?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@std@@@1@PEAU01@@Z @ 0x18000B0A8
----------
void __fastcall std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>::_Freenode<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>(__int64 a1, char *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::wstring::_Tidy_deallocate((__int64)(a2 + 32));
  std::_Deallocate<16,0>(a2, 0x50ui64);
}


==========

FUNCTION: ??$_Get_size_of_n@$0CA@@std@@YA_K_K@Z @ 0x18000B0D4
----------
unsigned __int64 __fastcall std::_Get_size_of_n<32>(unsigned __int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a1 > 0x7FFFFFFFFFFFFFFi64 )
    std::_Throw_bad_array_new_length();
  return 32 * a1;
}


==========

FUNCTION: ??$_Uninitialized_move@PEAV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@YAPEAV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@0@QEAV10@0PEAV10@AEAV?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@0@@Z @ 0x18000B100
----------
__int64 __fastcall std::_Uninitialized_move<std::wstring *>(__int64 a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = a1;
  if ( a1 != a2 )
  {
    do
    {
      std::_Default_allocator_traits<std::allocator<std::wstring>>::construct<std::wstring,std::wstring>(a1, a3, v4);
      a3 += 32i64;
      v4 = v5 + 32;
    }
    while ( v4 != v6 );
  }
  std::_Destroy_range<std::allocator<std::wstring>>(a3, a3);
  return a3;
}


==========

FUNCTION: ??$construct@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@AEAU12@@?$_Default_allocator_traits@V?$allocator@U?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@std@@@std@@@std@@SAXAEAV?$allocator@U?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@std@@@1@QEAU?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@1@AEAU31@@Z @ 0x18000B144
----------
_QWORD *__fastcall std::_Default_allocator_traits<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>::construct<std::pair<std::wstring const,cxl::PropertyList::Property>,std::pair<std::wstring const,cxl::PropertyList::Property> &>(__int64 a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  result = std::wstring::wstring((_QWORD *)a2, a3);
  *(_OWORD *)(a2 + 32) = *(_OWORD *)(a3 + 32);
  return result;
}


==========

FUNCTION: ??$construct@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEAV12@@?$_Default_allocator_traits@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@std@@@std@@SAXAEAV?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@1@QEAV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@1@AEAV31@@Z @ 0x18000B17C
----------
_QWORD *__fastcall std::_Default_allocator_traits<std::allocator<std::wstring>>::construct<std::wstring,std::wstring &>(__int64 a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return std::wstring::wstring((_QWORD *)a2, a3);
}


==========

FUNCTION: ??$construct@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V12@@?$_Default_allocator_traits@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@std@@@std@@SAXAEAV?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@1@QEAV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@1@$$QEAV31@@Z @ 0x18000B190
----------
__int64 __fastcall std::_Default_allocator_traits<std::allocator<std::wstring>>::construct<std::wstring,std::wstring>(__int64 a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  result = 0i64;
  *(_OWORD *)a2 = 0i64;
  *(_QWORD *)(a2 + 16) = 0i64;
  *(_QWORD *)(a2 + 24) = 0i64;
  *(_OWORD *)a2 = *(_OWORD *)a3;
  *(_OWORD *)(a2 + 16) = *(_OWORD *)(a3 + 16);
  *(_QWORD *)(a3 + 16) = 0i64;
  *(_QWORD *)(a3 + 24) = 7i64;
  *(_WORD *)a3 = 0;
  return result;
}


==========

FUNCTION: ??$for_each@V?$_Vector_const_iterator@V?$_Vector_val@U?$_Simple_types@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@std@@@std@@@std@@V_lambda_1703c254e6c52ab67ccadba2463ea9e5_@@@std@@YA?AV_lambda_1703c254e6c52ab67ccadba2463ea9e5_@@V?$_Vector_const_iterator@V?$_Vector_val@U?$_Simple_types@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@std@@@std@@@0@0V1@@Z @ 0x18000B1C8
----------
_QWORD *__fastcall std::for_each<std::_Vector_const_iterator<std::_Vector_val<std::_Simple_types<std::wstring>>>,_lambda_1703c254e6c52ab67ccadba2463ea9e5_>(_QWORD *a1, __int64 a2, __int64 a3, __int64 a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  for ( i = a2; i != a3; i += 32i64 )
  {
    for ( j = 0i64; j < *(_QWORD *)(i + 16); ++j )
    {
      v9 = std::_String_val<std::_Simple_types<wchar_t>>::_Myptr(i);
      v10 = *(_BYTE **)(a4 + 8);
      v11 = (_WORD *)v9 + j;
      if ( v10 == *(_BYTE **)(a4 + 16) )
      {
        std::vector<wchar_t>::_Emplace_reallocate<wchar_t const &>((const void **)a4, v10, v11);
      }
      else
      {
        *(_WORD *)v10 = *v11;
        *(_QWORD *)(a4 + 8) += 2i64;
      }
    }
    v13 = 0;
    std::vector<wchar_t>::push_back(a4, &v13);
  }
  result = a1;
  *a1 = a4;
  return result;
}


==========

FUNCTION: ??0?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@QEAA@AEBV01@@Z @ 0x18000B45C
----------
_QWORD *__fastcall std::wstring::wstring(_QWORD *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_OWORD *)a1 = 0i64;
  a1[2] = 0i64;
  a1[3] = 0i64;
  v3 = std::_String_val<std::_Simple_types<wchar_t>>::_Myptr(a2);
  std::wstring::_Construct<2,wchar_t const *>(a1, v3, *(_QWORD *)(v4 + 16));
  return a1;
}


==========

FUNCTION: ??0?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@QEAA@QEB_W@Z @ 0x18000B49C
----------
_QWORD *__fastcall std::wstring::wstring(_QWORD *a1, _WORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_OWORD *)a1 = 0i64;
  a1[2] = 0i64;
  a1[3] = 0i64;
  v3 = -1i64;
  do
    ++v3;
  while ( a2[v3] );
  std::wstring::_Construct<1,wchar_t const *>(a1, a2, v3);
  return a1;
}


==========

FUNCTION: ??0?$function@$$A6AXXZ@std@@QEAA@AEBV01@@Z @ 0x18000B4D8
----------
__int64 __fastcall std::function<void (void)>::function<void (void)>(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)(a1 + 56) = 0i64;
  v3 = *(__int64 (__fastcall ****)(_QWORD, __int64))(a2 + 56);
  if ( v3 )
    *(_QWORD *)(a1 + 56) = (**v3)(v3, a1);
  return a1;
}


==========

FUNCTION: ??0bad_alloc@std@@QEAA@AEBV01@@Z @ 0x18000B610
----------
std::bad_alloc *__fastcall std::bad_alloc::bad_alloc(std::bad_alloc *this, const struct std::bad_alloc *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::exception::exception(this, a2);
  *(_QWORD *)this = &std::bad_alloc::`vftable';
  return this;
}


==========

FUNCTION: ??0bad_array_new_length@std@@QEAA@AEBV01@@Z @ 0x18000B638
----------
std::bad_array_new_length *__fastcall std::bad_array_new_length::bad_array_new_length(std::bad_array_new_length *this, const struct std::bad_array_new_length *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::exception::exception(this, a2);
  *(_QWORD *)this = &std::bad_array_new_length::`vftable';
  return this;
}


==========

FUNCTION: ??0bad_array_new_length@std@@QEAA@XZ @ 0x18000B660
----------
std::bad_array_new_length *__fastcall std::bad_array_new_length::bad_array_new_length(std::bad_array_new_length *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *((_QWORD *)this + 2) = 0i64;
  *((_QWORD *)this + 1) = "bad array new length";
  *(_QWORD *)this = &std::bad_array_new_length::`vftable';
  return this;
}


==========

FUNCTION: ??1?$_Alloc_construct_ptr@V?$allocator@U?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@std@@@std@@@std@@QEAA@XZ @ 0x18000B744
----------
void __fastcall std::_Alloc_construct_ptr<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>::~_Alloc_construct_ptr<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v1 = *(void **)(a1 + 8);
  if ( v1 )
    std::_Deallocate<16,0>(v1, 0x50ui64);
}


==========

FUNCTION: ??1?$function@$$A6AXPEAUCLUSPROP_ULARGE_INTEGER@@@Z@std@@QEAA@XZ @ 0x18000B768
----------
__int64 __fastcall std::function<void (CLUSPROP_ULARGE_INTEGER *)>::~function<void (CLUSPROP_ULARGE_INTEGER *)>(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = *(_QWORD *)(a1 + 56);
  if ( !v3 )
    return result;
  LOBYTE(a2) = v3 != a1;
  result = (*(__int64 (__fastcall **)(__int64, __int64))(*(_QWORD *)v3 + 32i64))(v3, a2);
  *(_QWORD *)(a1 + 56) = 0i64;
  return result;
}


==========

FUNCTION: ??1?$_Tidy_guard@V?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@@std@@QEAA@XZ @ 0x18000B7A0
----------
void __fastcall std::_Tidy_guard<std::vector<std::wstring>>::~_Tidy_guard<std::vector<std::wstring>>(__int64 *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v1 = *a1;
  if ( v1 )
    std::vector<std::wstring>::_Tidy(v1);
}


==========

FUNCTION: ??1?$_Uninitialized_backout_al@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@std@@@std@@QEAA@XZ @ 0x18000B7C0
----------
void __fastcall std::_Uninitialized_backout_al<std::allocator<std::wstring>>::~_Uninitialized_backout_al<std::allocator<std::wstring>>(__int64 *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::_Destroy_range<std::allocator<std::wstring>>(*a1, a1[1]);
}


==========

FUNCTION: ??1?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@QEAA@XZ @ 0x18000B7D4
----------
// attributes: thunk
void __fastcall std::wstring::~wstring(std::wstring *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::wstring::_Tidy_deallocate((__int64)this);
}


==========

FUNCTION: ??1?$shared_ptr@$$CBVMiSession@mi@@@std@@QEAA@XZ @ 0x18000B7E0
----------
void __fastcall std::shared_ptr<mi::MiSession const>::~shared_ptr<mi::MiSession const>(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v1 = *(std::_Ref_count_base **)(a1 + 8);
  if ( v1 )
    std::_Ref_count_base::_Decref(v1);
}


==========

FUNCTION: ??1?$unique_ptr@VWindowsFeature@ServerManager@@U?$default_delete@VWindowsFeature@ServerManager@@@std@@@std@@QEAA@XZ @ 0x18000B800
----------
void __fastcall std::unique_ptr<ServerManager::WindowsFeature>::~unique_ptr<ServerManager::WindowsFeature>(ServerManager::WindowsFeature **a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v1 = *a1;
  if ( *a1 )
  {
    ServerManager::WindowsFeature::~WindowsFeature(*a1);
    operator delete(v1);
  }
}


==========

FUNCTION: ??$_Destroy_in_place@V?$vector@EV?$allocator@E@std@@@std@@@std@@YAXAEAV?$vector@EV?$allocator@E@std@@@0@@Z @ 0x18000B830
----------
// attributes: thunk
void __fastcall std::_Destroy_in_place<std::vector<unsigned char>>(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::vector<unsigned char>::_Tidy(a1);
}


==========

FUNCTION: ??1?$vector@_WV?$allocator@_W@std@@@std@@QEAA@XZ @ 0x18000B83C
----------
// attributes: thunk
void __fastcall std::vector<wchar_t>::~vector<wchar_t>(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::vector<wchar_t>::_Tidy(a1);
}


==========

FUNCTION: ??4?$vector@EV?$allocator@E@std@@@std@@QEAAAEAV01@AEBV01@@Z @ 0x18000B964
----------
void **__fastcall std::vector<unsigned char>::operator=(void **a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a1 != (void **)a2 )
    std::vector<unsigned char>::_Assign_counted_range<unsigned char const *>(a1, *(char **)a2, *(_QWORD *)(a2 + 8) - *(_QWORD *)a2);
  return a1;
}


==========

FUNCTION: ??R?$_Func_class@X$$V@std@@QEBAXXZ @ 0x18000C9C4
----------
__int64 __fastcall std::_Func_class<void,>::operator()(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v1 = *(_QWORD *)(a1 + 56);
  if ( v1 )
    return (*(__int64 (__fastcall **)(__int64))(*(_QWORD *)v1 + 16i64))(v1);
  std::_Xbad_function_call();
  __debugbreak();
  return (*(__int64 (__fastcall **)(__int64))(*(_QWORD *)v1 + 16i64))(v1);
}


==========

FUNCTION: ?_Adjust_manually_vector_aligned@std@@YAXAEAPEAXAEA_K@Z @ 0x18000CD18
----------
void __fastcall std::_Adjust_manually_vector_aligned(void **a1, unsigned __int64 *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a2 += 39i64;
  v2 = (_BYTE *)*((_QWORD *)*a1 - 1);
  if ( (unsigned __int64)((_BYTE *)*a1 - v2 - 8) > 0x1F )
  {
    _o__invalid_parameter_noinfo_noreturn();
    __debugbreak();
    JUMPOUT(0x18000CD4Ai64);
  }
  *a1 = v2;
}


==========

FUNCTION: ?_Buy_raw@?$vector@EV?$allocator@E@std@@@std@@AEAAX_K@Z @ 0x18000CD50
----------
char *__fastcall std::vector<unsigned char>::_Buy_raw(_QWORD *a1, size_t a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v6 = a2;
  v4 = std::_Allocate_at_least_helper<std::allocator<unsigned char>>((__int64)a1, &v6);
  *a1 = v4;
  a1[1] = v4;
  result = (char *)v4 + a2;
  a1[2] = result;
  return result;
}


==========

FUNCTION: ?_Calculate_growth@?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEBA_K_K@Z @ 0x18000CD90
----------
__int64 __fastcall std::wstring::_Calculate_growth(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(_QWORD *)(a1 + 24);
  v3 = a2 | 7;
  v4 = 0x7FFFFFFFFFFFFFFEi64;
  if ( (a2 | 7ui64) > 0x7FFFFFFFFFFFFFFEi64 )
    return v4;
  v5 = v2 >> 1;
  if ( v2 > 0x7FFFFFFFFFFFFFFEi64 - (v2 >> 1) )
    return v4;
  v4 = v5 + v2;
  if ( v3 >= v5 + v2 )
    v4 = v3;
  return v4;
}


==========

FUNCTION: ?_Calculate_growth@?$vector@EV?$allocator@E@std@@@std@@AEBA_K_K@Z @ 0x18000CDD4
----------
unsigned __int64 __fastcall std::vector<unsigned char>::_Calculate_growth(_QWORD *a1, unsigned __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  result = 0x7FFFFFFFFFFFFFFFi64;
  v3 = a1[2] - *a1;
  v4 = v3 >> 1;
  if ( v3 > 0x7FFFFFFFFFFFFFFFi64 - (v3 >> 1) )
    return result;
  result = v4 + v3;
  if ( v4 + v3 < a2 )
    result = a2;
  return result;
}


==========

FUNCTION: ?_Calculate_growth@?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@AEBA_K_K@Z @ 0x18000CE0C
----------
unsigned __int64 __fastcall std::vector<std::wstring>::_Calculate_growth(_QWORD *a1, unsigned __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  result = 0x7FFFFFFFFFFFFFFi64;
  v3 = (__int64)(a1[2] - *a1) /*signed*/>> 5;
  v4 = v3 >> 1;
  if ( v3 > 0x7FFFFFFFFFFFFFFi64 - (v3 >> 1) )
    return result;
  result = v4 + v3;
  if ( v4 + v3 < a2 )
    result = a2;
  return result;
}


==========

FUNCTION: ?_Calculate_growth@?$vector@_WV?$allocator@_W@std@@@std@@AEBA_K_K@Z @ 0x18000CE48
----------
unsigned __int64 __fastcall std::vector<wchar_t>::_Calculate_growth(_QWORD *a1, unsigned __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  result = 0x7FFFFFFFFFFFFFFFi64;
  v3 = (__int64)(a1[2] - *a1) /*signed*/>> 1;
  v4 = v3 >> 1;
  if ( v3 > 0x7FFFFFFFFFFFFFFFi64 - (v3 >> 1) )
    return result;
  result = v4 + v3;
  if ( v4 + v3 < a2 )
    result = a2;
  return result;
}


==========

FUNCTION: ?_Change_array@?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@AEAAXQEAV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@2@_K1@Z @ 0x18000CE80
----------
void __fastcall std::vector<std::wstring>::_Change_array(__int64 a1, __int64 a2, __int64 a3, __int64 a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v6 = *(void **)a1;
  if ( v6 )
  {
    std::_Destroy_range<std::allocator<std::wstring>>((__int64)v6, *(_QWORD *)(a1 + 8));
    std::_Deallocate<16,0>(*(void **)a1, (*(_QWORD *)(a1 + 16) - *(_QWORD *)a1) & 0xFFFFFFFFFFFFFFE0ui64);
  }
  *(_QWORD *)a1 = a2;
  *(_QWORD *)(a1 + 8) = a2 + 32 * a3;
  *(_QWORD *)(a1 + 16) = a2 + 32 * a4;
}


==========

FUNCTION: ?_Change_array@?$vector@_WV?$allocator@_W@std@@@std@@AEAAXQEA_W_K1@Z @ 0x18000CEFC
----------
__int64 __fastcall std::vector<wchar_t>::_Change_array(__int64 a1, __int64 a2, __int64 a3, __int64 a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v6 = *(void **)a1;
  if ( v6 )
    std::_Deallocate<16,0>(v6, 2 * ((__int64)(*(_QWORD *)(a1 + 16) - (_QWORD)v6) /*signed*/>> 1));
  *(_QWORD *)a1 = a2;
  *(_QWORD *)(a1 + 8) = a2 + 2 * a3;
  result = a2 + 2 * a4;
  *(_QWORD *)(a1 + 16) = result;
  return result;
}


==========

FUNCTION: ?_Clear_and_reserve_geometric@?$vector@EV?$allocator@E@std@@@std@@AEAAX_K@Z @ 0x18000CF68
----------
char *__fastcall std::vector<unsigned char>::_Clear_and_reserve_geometric(void **a1, unsigned __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a2 > 0x7FFFFFFFFFFFFFFFi64 )
    std::vector<wchar_t>::_Xlength();
  v3 = std::vector<unsigned char>::_Calculate_growth(a1, a2);
  if ( !*a1 )
    return std::vector<unsigned char>::_Buy_raw(a1, v3);
  std::_Deallocate<16,0>(*a1, (_BYTE *)a1[2] - (_BYTE *)*a1);
  *a1 = 0i64;
  a1[1] = 0i64;
  a1[2] = 0i64;
  return std::vector<unsigned char>::_Buy_raw(a1, v3);
}


==========

FUNCTION: ?_Deallocate_for_capacity@?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@CAXAEAV?$allocator@_W@2@QEA_W_K@Z @ 0x18000D130
----------
void __fastcall std::wstring::_Deallocate_for_capacity(__int64 a1, void *a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::_Deallocate<16,0>(a2, 2 * a3 + 2);
}


==========

FUNCTION: ?_Decref@_Ref_count_base@std@@QEAAXXZ @ 0x18000D148
----------
void __fastcall std::_Ref_count_base::_Decref(std::_Ref_count_base *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( _InterlockedExchangeAdd((volatile signed __int32 *)this + 2, 0xFFFFFFFF) == 1 )
  {
    (**(void (__fastcall ***)(std::_Ref_count_base *))this)(this);
    std::_Ref_count_base::_Decwref(this);
  }
}


==========

FUNCTION: ?_Decwref@_Ref_count_base@std@@QEAAXXZ @ 0x18000D180
----------
void __fastcall std::_Ref_count_base::_Decwref(std::_Ref_count_base *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( _InterlockedExchangeAdd((volatile signed __int32 *)this + 3, 0xFFFFFFFF) == 1 )
    (*(void (__fastcall **)(std::_Ref_count_base *))(*(_QWORD *)this + 8i64))(this);
}


==========

FUNCTION: ?_Get@?$_Func_impl_no_alloc@UResultsFunctor@ServerManager@@XAEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEBVMiValue@mi@@@std@@EEBAPEBXXZ @ 0x18000D2C0
----------
__int64 __fastcall std::_Func_impl_no_alloc<ServerManager::ResultsFunctor,void,std::wstring const &,mi::MiValue const &>::_Get(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return a1 + 8;
}


==========

FUNCTION: ?_Large_mode_engaged@?$_String_val@U?$_Simple_types@_W@std@@@std@@QEBA_NXZ @ 0x18000D2CC
----------
bool __fastcall std::_String_val<std::_Simple_types<wchar_t>>::_Large_mode_engaged(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return *(_QWORD *)(a1 + 24) > 7ui64;
}


==========

FUNCTION: ?_Min@?$_Tree_val@U?$_Tree_simple_types@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@@std@@@std@@SAPEAU?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@2@PEAU32@@Z @ 0x18000D2DC
----------
_QWORD *__fastcall std::_Tree_val<std::_Tree_simple_types<std::pair<std::wstring const,cxl::PropertyList::Property>>>::_Min(_QWORD *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v1 = (__int64 *)*a1;
  if ( !*(_BYTE *)(*a1 + 25i64) )
  {
    do
    {
      a1 = v1;
      v1 = (__int64 *)*v1;
    }
    while ( !*((_BYTE *)v1 + 25) );
  }
  return a1;
}


==========

FUNCTION: ?_Myptr@?$_String_val@U?$_Simple_types@_W@std@@@std@@QEBAPEB_WXZ @ 0x18000D300
----------
_QWORD *__fastcall std::_String_val<std::_Simple_types<wchar_t>>::_Myptr(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( std::_String_val<std::_Simple_types<wchar_t>>::_Large_mode_engaged(a1) )
    v1 = (_QWORD *)*v1;
  return v1;
}


==========

FUNCTION: ?_Throw_bad_array_new_length@std@@YAXXZ @ 0x18000D3D0
----------
void __noreturn std::_Throw_bad_array_new_length(void)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  pExceptionObject[2] = 0i64;
  pExceptionObject[1] = (__int64)"bad array new length";
  pExceptionObject[0] = (__int64)&std::bad_array_new_length::`vftable';
  CxxThrowException_0(pExceptionObject, (_ThrowInfo *)&TI3_AVbad_array_new_length_std__);
}


==========

FUNCTION: ?_Tidy@?$vector@EV?$allocator@E@std@@@std@@AEAAXXZ @ 0x18000D40C
----------
void __fastcall std::vector<unsigned char>::_Tidy(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(void **)a1;
  if ( v2 )
  {
    std::_Deallocate<16,0>(v2, *(_QWORD *)(a1 + 16) - (_QWORD)v2);
    *(_QWORD *)a1 = 0i64;
    *(_QWORD *)(a1 + 8) = 0i64;
    *(_QWORD *)(a1 + 16) = 0i64;
  }
}


==========

FUNCTION: ?_Tidy@?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@AEAAXXZ @ 0x18000D444
----------
void __fastcall std::vector<std::wstring>::_Tidy(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(void **)a1;
  if ( v2 )
  {
    std::_Destroy_range<std::allocator<std::wstring>>((__int64)v2, *(_QWORD *)(a1 + 8));
    std::_Deallocate<16,0>(*(void **)a1, (*(_QWORD *)(a1 + 16) - *(_QWORD *)a1) & 0xFFFFFFFFFFFFFFE0ui64);
    *(_QWORD *)a1 = 0i64;
    *(_QWORD *)(a1 + 8) = 0i64;
    *(_QWORD *)(a1 + 16) = 0i64;
  }
}


==========

FUNCTION: ?_Tidy@?$vector@_WV?$allocator@_W@std@@@std@@AEAAXXZ @ 0x18000D48C
----------
void __fastcall std::vector<wchar_t>::_Tidy(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(void **)a1;
  if ( v2 )
  {
    std::_Deallocate<16,0>(v2, 2 * ((__int64)(*(_QWORD *)(a1 + 16) - (_QWORD)v2) /*signed*/>> 1));
    *(_QWORD *)a1 = 0i64;
    *(_QWORD *)(a1 + 8) = 0i64;
    *(_QWORD *)(a1 + 16) = 0i64;
  }
}


==========

FUNCTION: ?_Tidy_deallocate@?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEAAXXZ @ 0x18000D4CC
----------
void __fastcall std::wstring::_Tidy_deallocate(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( std::_String_val<std::_Simple_types<wchar_t>>::_Large_mode_engaged(a1) )
    std::wstring::_Deallocate_for_capacity(v2, *(void **)v2, *(_QWORD *)(v2 + 24));
  *(_QWORD *)(a1 + 16) = 0i64;
  *(_WORD *)a1 = 0;
  *(_QWORD *)(a1 + 24) = 7i64;
}


==========

FUNCTION: ?_Xlen_string@std@@YAXXZ @ 0x18000D51C
----------
void __noreturn std::_Xlen_string(void)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::_Xlength_error("string too long");
}


==========

FUNCTION: ?_Xlength@?$vector@_WV?$allocator@_W@std@@@std@@CAXXZ @ 0x18000D53C
----------
void __noreturn std::vector<wchar_t>::_Xlength()
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::_Xlength_error("vector too long");
}


==========

FUNCTION: ?clear@?$_Tree@V?$_Tmap_traits@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@UCaseInsensitive_LessThan@5@V?$allocator@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@@2@$0A@@std@@@std@@QEAAXXZ @ 0x18000D55C
----------
void __fastcall std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::clear(_QWORD *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v1 = (_QWORD *)*a1;
  std::_Tree_val<std::_Tree_simple_types<std::pair<std::wstring const,cxl::PropertyList::Property>>>::_Erase_tree_and_orphan<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>((__int64)a1, (__int64)a1, *(__int64 **)(*a1 + 8i64));
  v1[1] = v1;
  *v1 = v1;
  v1[2] = v1;
  a1[1] = 0i64;
}


==========

FUNCTION: ?push_back@?$vector@_WV?$allocator@_W@std@@@std@@QEAAX$$QEA_W@Z @ 0x18000E00C
----------
unsigned __int64 __fastcall std::vector<wchar_t>::push_back(__int64 a1, _WORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = *(_BYTE **)(a1 + 8);
  if ( v3 == *(_BYTE **)(a1 + 16) )
    return (unsigned __int64)std::vector<wchar_t>::_Emplace_reallocate<wchar_t const &>((const void **)a1, v3, a2);
  result = (unsigned __int16)*a2;
  *(_WORD *)v3 = result;
  *(_QWORD *)(a1 + 8) += 2i64;
  return result;
}


==========

FUNCTION: ??$?0V?$_String_iterator@V?$_String_val@U?$_Simple_types@_W@std@@@std@@@std@@$0A@@?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@QEAA@V?$_String_iterator@V?$_String_val@U?$_Simple_types@_W@std@@@std@@@1@0AEBV?$allocator@D@1@@Z @ 0x18000E96C
----------
__int64 __fastcall std::string::string(__int64 a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_OWORD *)a1 = 0i64;
  *(_QWORD *)(a1 + 16) = 0i64;
  *(_QWORD *)(a1 + 24) = 0i64;
  if ( a2 == a3 )
  {
    *(_QWORD *)(a1 + 24) = 15i64;
    *(_BYTE *)a1 = 0;
  }
  else
  {
    std::string::_Construct_from_iter<wchar_t *,wchar_t *,unsigned __int64>((_QWORD *)a1, (_BYTE *)a2, (_BYTE *)a3, (a3 - a2) /*signed*/>> 1);
  }
  return a1;
}


==========

FUNCTION: ??$?6_WU?$char_traits@_W@std@@@std@@YAAEAV?$basic_ostream@_WU?$char_traits@_W@std@@@0@AEAV10@PEB_W@Z @ 0x18000E9B4
----------
__int64 __fastcall std::operator<<<wchar_t,std::char_traits<wchar_t>>(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = 0;
  v5 = -1i64;
  do
    ++v5;
  while ( *(_WORD *)(a2 + 2 * v5) );
  if ( std::ios_base::width((std::ios_base *)(a1 + *(int *)(*(_QWORD *)a1 + 4i64))) /*signed*/<= 0 || std::ios_base::width((std::ios_base *)(a1 + *(int *)(*(_QWORD *)a1 + 4i64))) /*signed*/<= v5 )
    v6 = 0i64;
  else
    v6 = std::ios_base::width((std::ios_base *)(a1 + *(int *)(*(_QWORD *)a1 + 4i64))) - v5;
  std::wostream::sentry::sentry((__int64)v15, a1);
  if ( v15[8] )
  {
    if ( (std::ios_base::flags((std::ios_base *)(a1 + *(int *)(*(_QWORD *)a1 + 4i64))) & 0x1C0) != 64 )
    {
      while ( v6 /*signed*/> 0 )
      {
        v7 = std::wios::rdbuf(a1 + *(int *)(*(_QWORD *)a1 + 4i64));
        v8 = std::wios::fill(a1 + *(int *)(*(_QWORD *)a1 + 4i64));
        v9 = std::wstreambuf::sputc(v7, v8);
        if ( std::_WChar_traits<wchar_t>::eq_int_type(0xFFFF, v9) )
        {
          v4 = 4;
          goto LABEL_18;
        }
        --v6;
      }
    }
    v10 = std::wios::rdbuf(a1 + *(int *)(*(_QWORD *)a1 + 4i64));
    if ( std::wstreambuf::sputn(v10, a2, v5) == v5 )
    {
      while ( v6 /*signed*/> 0 )
      {
        v11 = std::wios::rdbuf(a1 + *(int *)(*(_QWORD *)a1 + 4i64));
        v12 = std::wios::fill(a1 + *(int *)(*(_QWORD *)a1 + 4i64));
        v13 = std::wstreambuf::sputc(v11, v12);
        if ( std::_WChar_traits<wchar_t>::eq_int_type(0xFFFF, v13) )
          goto LABEL_17;
        --v6;
      }
    }
    else
    {
LABEL_17:
      v4 = 4;
    }
LABEL_18:
    std::ios_base::width((std::ios_base *)(a1 + *(int *)(*(_QWORD *)a1 + 4i64)), 0i64);
  }
  else
  {
    v4 = 4;
  }
  std::wios::setstate(a1 + *(int *)(*(_QWORD *)a1 + 4i64), v4, 0i64);
  std::wostream::sentry::~sentry((std::wostream::sentry *)v15);
  return a1;
}


==========

FUNCTION: ??$?6_WU?$char_traits@_W@std@@V?$allocator@_W@1@@std@@YAAEAV?$basic_ostream@_WU?$char_traits@_W@std@@@0@AEAV10@AEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@0@@Z @ 0x18000EBEC
----------
__int64 __fastcall std::operator<<<wchar_t>(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = std::_String_val<std::_Simple_types<wchar_t>>::_Myptr(a2);
  return std::_Insert_string<wchar_t,std::char_traits<wchar_t>,unsigned __int64>(v4, (__int64)v2, *(_QWORD *)(v3 + 16));
}


==========

FUNCTION: ??$_Allocate_at_least_helper@V?$allocator@VMiInstance@mi@@@std@@@std@@YAPEAVMiInstance@mi@@AEAV?$allocator@VMiInstance@mi@@@0@AEA_K@Z @ 0x18000F25C
----------
_QWORD *__fastcall std::_Allocate_at_least_helper<std::allocator<mi::MiInstance>>(__int64 a1, _QWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( *a2 > 0x492492492492492ui64 )
    std::_Throw_bad_array_new_length();
  return std::_Allocate<16,std::_Default_allocate_traits,0>(56i64 * *a2);
}


==========

FUNCTION: ??$_Construct@$01PEBD@?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@AEAAXQEBD_K@Z @ 0x18000F28C
----------
__int64 __fastcall std::string::_Construct<2,char const *>(_QWORD *a1, _OWORD *a2, unsigned __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  result = 0x7FFFFFFFFFFFFFFFi64;
  if ( a3 > 0x7FFFFFFFFFFFFFFFi64 )
    std::_Xlen_string();
  a1[3] = 15i64;
  if ( a3 > 0xF )
  {
    v7 = std::string::_Calculate_growth((__int64)a1, a3);
    v8 = std::_Allocate<16,std::_Default_allocate_traits,0>(v7 + 1);
    *a1 = v8;
    a1[2] = a3;
    a1[3] = v7;
    result = (__int64)memcpy_0(v8, a2, a3 + 1);
  }
  else
  {
    a1[2] = a3;
    *(_OWORD *)a1 = *a2;
  }
  return result;
}


==========

FUNCTION: ??$_Construct_from_iter@PEA_WPEA_W_K@?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@AEAAXPEA_WQEA_W_K@Z @ 0x18000F324
----------
__int64 __fastcall std::string::_Construct_from_iter<wchar_t *,wchar_t *,unsigned __int64>(_QWORD *a1, _BYTE *a2, _BYTE *a3, unsigned __int64 a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  a1[2] = 0i64;
  a1[3] = 15i64;
  if ( a4 > 0x7FFFFFFFFFFFFFFFi64 )
    std::_Xlen_string();
  if ( a4 > 0xF )
  {
    v7 = std::string::_Calculate_growth((__int64)a1, a4);
    *a1 = std::_Allocate<16,std::_Default_allocate_traits,0>(v7 + 1);
    a1[3] = v7;
  }
  while ( 1 )
  {
    v8 = std::_String_val<std::_Simple_types<char>>::_Myptr((__int64)a1);
    if ( a2 == a3 )
      break;
    *((_BYTE *)v8 + v10) = *a2;
    ++a1[2];
    a2 += 2;
  }
  v12 = 0i64;
  *((_BYTE *)v8 + v9) = 0;
  return std::_Tidy_deallocate_guard<std::string>::~_Tidy_deallocate_guard<std::string>(&v12);
}


==========

FUNCTION: ??$_Destroy_range@V?$allocator@VMiInstance@mi@@@std@@@std@@YAXPEAVMiInstance@mi@@QEAV12@AEAV?$allocator@VMiInstance@mi@@@0@@Z @ 0x18000F3E4
----------
void __fastcall std::_Destroy_range<std::allocator<mi::MiInstance>>(mi::MiInstance *this, mi::MiInstance *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( this != a2 )
  {
    v3 = this;
    do
      mi::MiInstance::~MiInstance(v3++);
    while ( v3 != a2 );
  }
}


==========

FUNCTION: ??$_Emplace_reallocate@AEAPEA_W@?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@AEAAPEAV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@1@QEAV21@AEAPEA_W@Z @ 0x18000F41C
----------
char *__fastcall std::vector<std::wstring>::_Emplace_reallocate<wchar_t * &>(__int64 *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v6 = a2 - *a1;
  v7 = (a1[1] - *a1) /*signed*/>> 5;
  if ( v7 == 0x7FFFFFFFFFFFFFFi64 )
    std::vector<wchar_t>::_Xlength();
  v8 = v7 + 1;
  v17 = std::vector<std::wstring>::_Calculate_growth(a1, v7 + 1);
  v10 = std::_Allocate_at_least_helper<std::allocator<std::wstring>>(v9, &v17);
  v11 = (_QWORD *)((char *)v10 + (v6 & 0xFFFFFFFFFFFFFFE0ui64));
  v18 = v11 + 4;
  std::_Default_allocator_traits<std::allocator<std::wstring>>::construct<std::wstring,wchar_t * &>(v12, v11, (_WORD **)a3);
  v13 = a1[1];
  v14 = (__int64)v10;
  v15 = *a1;
  if ( a2 != v13 )
  {
    std::_Uninitialized_move<std::wstring *>(v15, a2, (__int64)v10);
    v14 = (__int64)(v11 + 4);
    v13 = a1[1];
    v15 = a2;
  }
  std::_Uninitialized_move<std::wstring *>(v15, v13, v14);
  std::vector<std::wstring>::_Change_array((__int64)a1, (__int64)v10, v8, v17);
  return (char *)v11;
}


==========

FUNCTION: ??$_Emplace_reallocate@VMiInstance@mi@@@?$vector@VMiInstance@mi@@V?$allocator@VMiInstance@mi@@@std@@@std@@AEAAPEAVMiInstance@mi@@QEAV23@$$QEAV23@@Z @ 0x18000F504
----------
__int64 __fastcall std::vector<mi::MiInstance>::_Emplace_reallocate<mi::MiInstance>(__int64 *a1, mi::MiInstance *a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v20 = a3;
  v5 = (__int64)&a2->mi_object_00[-*a1] /*signed*// 56;
  v6 = 0x6DB6DB6DB6DB6DB7i64 * ((a1[1] - *a1) /*signed*/>> 3);
  v7 = 0x492492492492492i64;
  if ( v6 == 0x492492492492492i64 )
    std::_Xlength_error("vector too long");
  v8 = v6 + 1;
  v9 = 0x6DB6DB6DB6DB6DB7i64 * ((a1[2] - *a1) /*signed*/>> 3);
  v10 = v9 >> 1;
  if ( v9 <= 0x492492492492492i64 - (v9 >> 1) )
  {
    v7 = v10 + v9;
    if ( v10 + v9 < v8 )
      v7 = v8;
  }
  v19 = v7;
  v18 = v7;
  v11 = (mi::MiInstance *)std::_Allocate_at_least_helper<std::allocator<mi::MiInstance>>(v9, &v18);
  v12 = (__int64)v11[v5].mi_object_00;
  v21 = v12 + 56;
  std::_Default_allocator_traits<std::allocator<mi::MiInstance>>::construct<mi::MiInstance,mi::MiInstance>(v13, v12, v20);
  v18 = v12;
  v14 = (mi::MiInstance *)a1[1];
  v15 = v11;
  v16 = *a1;
  if ( a2 != v14 )
  {
    std::_Uninitialized_move<mi::MiInstance *>(v16, (__int64)a2, v11);
    v18 = (__int64)v11;
    v15 = (mi::MiInstance *)(v12 + 56);
    v14 = (mi::MiInstance *)a1[1];
    v16 = (__int64)a2;
  }
  std::_Uninitialized_move<mi::MiInstance *>(v16, (__int64)v14, v15);
  if ( *a1 )
  {
    std::_Destroy_range<std::allocator<mi::MiInstance>>((mi::MiInstance *)*a1, (mi::MiInstance *)a1[1]);
    std::_Deallocate<16,0>((void *)*a1, 8 * ((a1[2] - *a1) /*signed*/>> 3));
  }
  *a1 = (__int64)v11;
  a1[1] = (__int64)v11[v8].mi_object_00;
  a1[2] = (__int64)v11[v7].mi_object_00;
  return v12;
}


==========

FUNCTION: ??$_Insert_string@_WU?$char_traits@_W@std@@_K@std@@YAAEAV?$basic_ostream@_WU?$char_traits@_W@std@@@0@AEAV10@QEB_W_K@Z @ 0x18000F688
----------
__int64 __fastcall std::_Insert_string<wchar_t,std::char_traits<wchar_t>,unsigned __int64>(__int64 a1, __int64 a2, unsigned __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v6 = 0;
  if ( std::ios_base::width((std::ios_base *)(a1 + *(int *)(*(_QWORD *)a1 + 4i64))) /*signed*/<= 0 || std::ios_base::width((std::ios_base *)(a1 + *(int *)(*(_QWORD *)a1 + 4i64))) <= a3 )
    v7 = 0i64;
  else
    v7 = std::ios_base::width((std::ios_base *)(a1 + *(int *)(*(_QWORD *)a1 + 4i64))) - a3;
  std::wostream::sentry::sentry((__int64)v16, a1);
  if ( v16[8] )
  {
    if ( (std::ios_base::flags((std::ios_base *)(a1 + *(int *)(*(_QWORD *)a1 + 4i64))) & 0x1C0) != 64 )
    {
      while ( v7 )
      {
        v8 = std::wios::rdbuf(a1 + *(int *)(*(_QWORD *)a1 + 4i64));
        v9 = std::wios::fill(a1 + *(int *)(*(_QWORD *)a1 + 4i64));
        v10 = std::wstreambuf::sputc(v8, v9);
        if ( std::_WChar_traits<wchar_t>::eq_int_type(0xFFFF, v10) )
        {
          v6 = 4;
          goto LABEL_13;
        }
        --v7;
      }
    }
    v11 = std::wios::rdbuf(a1 + *(int *)(*(_QWORD *)a1 + 4i64));
    if ( std::wstreambuf::sputn(v11, a2, a3) == a3 )
    {
LABEL_13:
      while ( v7 )
      {
        v12 = std::wios::rdbuf(a1 + *(int *)(*(_QWORD *)a1 + 4i64));
        v13 = std::wios::fill(a1 + *(int *)(*(_QWORD *)a1 + 4i64));
        v14 = std::wstreambuf::sputc(v12, v13);
        if ( std::_WChar_traits<wchar_t>::eq_int_type(0xFFFF, v14) )
          goto LABEL_15;
        --v7;
      }
    }
    else
    {
LABEL_15:
      v6 |= 4u;
    }
    std::ios_base::width((std::ios_base *)(a1 + *(int *)(*(_QWORD *)a1 + 4i64)), 0i64);
  }
  else
  {
    v6 = 4;
  }
  std::wios::setstate(a1 + *(int *)(*(_QWORD *)a1 + 4i64), v6, 0i64);
  std::wostream::sentry::~sentry((std::wostream::sentry *)v16);
  return a1;
}


==========

FUNCTION: ??$_Reallocate_for@V_lambda_3fa8b2c8193a0f3144fc4b1b8f243931_@@PEB_W@?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEAAAEAV01@_KV_lambda_3fa8b2c8193a0f3144fc4b1b8f243931_@@PEB_W@Z @ 0x18000F8A4
----------
__int64 __fastcall std::wstring::_Reallocate_for<_lambda_3fa8b2c8193a0f3144fc4b1b8f243931_,wchar_t const *>(__int64 a1, unsigned __int64 a2, __int64 a3, const void *a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a2 > 0x7FFFFFFFFFFFFFFEi64 )
    std::_Xlen_string();
  v7 = *(_QWORD *)(a1 + 24);
  v8 = std::wstring::_Calculate_growth(a1, a2);
  if ( (unsigned __int64)(v8 + 1) > 0x7FFFFFFFFFFFFFFFi64 )
    std::_Throw_bad_array_new_length();
  v9 = std::_Allocate<16,std::_Default_allocate_traits,0>(2 * (v8 + 1));
  *(_QWORD *)(a1 + 16) = a2;
  v10 = 2 * a2;
  *(_QWORD *)(a1 + 24) = v8;
  v11 = v9;
  memcpy_0(v9, a4, v10);
  *(_WORD *)((char *)v11 + v10) = 0;
  if ( v7 > 7 )
    std::_Deallocate<16,0>(*(void **)a1, 2 * v7 + 2);
  *(_QWORD *)a1 = v11;
  return a1;
}


==========

FUNCTION: ??$_Reallocate_grow_by@V_lambda_65e615be2a453ca0576c979606f46740_@@PEBD_K@?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@AEAAAEAV01@_KV_lambda_65e615be2a453ca0576c979606f46740_@@PEBD_K@Z @ 0x18000F96C
----------
void *__fastcall std::string::_Reallocate_grow_by<_lambda_65e615be2a453ca0576c979606f46740_,char const *,unsigned __int64>(void *Src, unsigned __int64 a2, __int64 a3, const void *a4, size_t Size)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v5 = *((_QWORD *)Src + 2);
  if ( 0x7FFFFFFFFFFFFFFFi64 - v5 < a2 )
    std::_Xlen_string();
  v8 = *((_QWORD *)Src + 3);
  v9 = v5 + a2;
  v10 = std::string::_Calculate_growth((__int64)Src, v5 + a2);
  v11 = std::_Allocate<16,std::_Default_allocate_traits,0>(v10 + 1);
  *((_QWORD *)Src + 2) = v9;
  *((_QWORD *)Src + 3) = v10;
  v12 = (char *)v11 + v5;
  if ( v8 <= 0xF )
  {
    memcpy_0(v11, Src, v5);
    memcpy_0(v12, a4, Size);
    v12[Size] = 0;
  }
  else
  {
    v13 = *(void **)Src;
    memcpy_0(v11, *(const void **)Src, v5);
    memcpy_0(v12, a4, Size);
    v12[Size] = 0;
    std::_Deallocate<16,0>(v13, v8 + 1);
  }
  *(_QWORD *)Src = v11;
  return Src;
}


==========

FUNCTION: ??$_Uninitialized_move@PEAVMiInstance@mi@@V?$allocator@VMiInstance@mi@@@std@@@std@@YAPEAVMiInstance@mi@@QEAV12@0PEAV12@AEAV?$allocator@VMiInstance@mi@@@0@@Z @ 0x18000FA5C
----------
mi::MiInstance *__fastcall std::_Uninitialized_move<mi::MiInstance *>(__int64 a1, __int64 a2, mi::MiInstance *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = a1;
  if ( a1 != a2 )
  {
    do
    {
      std::_Default_allocator_traits<std::allocator<mi::MiInstance>>::construct<mi::MiInstance,mi::MiInstance>(a1, (__int64)a3++, v4);
      v4 = v5 + 56;
    }
    while ( v4 != v6 );
  }
  std::_Destroy_range<std::allocator<mi::MiInstance>>(a3, a3);
  return a3;
}


==========

FUNCTION: ??$construct@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEAPEA_W@?$_Default_allocator_traits@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@std@@@std@@SAXAEAV?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@1@QEAV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@1@AEAPEA_W@Z @ 0x18000FAA4
----------
_QWORD *__fastcall std::_Default_allocator_traits<std::allocator<std::wstring>>::construct<std::wstring,wchar_t * &>(__int64 a1, _QWORD *a2, _WORD **a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return std::wstring::wstring(a2, *a3);
}


==========

FUNCTION: ??$construct@VMiInstance@mi@@V12@@?$_Default_allocator_traits@V?$allocator@VMiInstance@mi@@@std@@@std@@SAXAEAV?$allocator@VMiInstance@mi@@@1@QEAVMiInstance@mi@@$$QEAV34@@Z @ 0x18000FAB8
----------
__int64 __fastcall std::_Default_allocator_traits<std::allocator<mi::MiInstance>>::construct<mi::MiInstance,mi::MiInstance>(__int64 a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return mi::MiInstance::MiInstance(a2, a3);
}


==========

FUNCTION: ??$endl@_WU?$char_traits@_W@std@@@std@@YAAEAV?$basic_ostream@_WU?$char_traits@_W@std@@@0@AEAV10@@Z @ 0x18000FAD0
----------
__int64 __fastcall std::endl<wchar_t,std::char_traits<wchar_t>>(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  LOBYTE(a2) = 10;
  v3 = std::wios::widen(a1 + *(int *)(*(_QWORD *)a1 + 4i64), a2);
  std::wostream::put(a1, v3);
  std::wostream::flush(a1);
  return a1;
}


==========

FUNCTION: ??0?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@QEAA@AEBV01@@Z @ 0x18000FB60
----------
_QWORD *__fastcall std::string::string(_QWORD *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_OWORD *)a1 = 0i64;
  a1[2] = 0i64;
  a1[3] = 0i64;
  v4 = std::_String_val<std::_Simple_types<char>>::_Myptr(a2);
  std::string::_Construct<2,char const *>(a1, v4, *(_QWORD *)(v5 + 16));
  return a1;
}


==========

FUNCTION: ??0?$basic_stringbuf@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@QEAA@H@Z @ 0x18000FBA0
----------
__int64 __fastcall std::wstringbuf::wstringbuf(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::wstreambuf::wstreambuf();
  *(_QWORD *)(a1 + 104) = 0i64;
  *(_DWORD *)(a1 + 112) = 0;
  *(_QWORD *)a1 = &std::wstringbuf::`vftable';
  return a1;
}


==========

FUNCTION: ??0?$basic_stringstream@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@QEAA@XZ @ 0x18000FBD8
----------
_QWORD *__fastcall std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>(_QWORD *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::`vbtable'{for `std::wistream'};
  a1[2] = &std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::`vbtable'{for `std::wostream'};
  std::wios::wios(a1 + 19);
  std::wiostream::basic_iostream<wchar_t>(a1, a1 + 3, 0i64);
  *(_QWORD *)((char *)a1 + *(int *)(*a1 + 4i64)) = &std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::`vftable';
  *(_DWORD *)((char *)a1 + *(int *)(*a1 + 4i64) - 4) = *(_DWORD *)(*a1 + 4i64) - 152;
  std::wstringbuf::wstringbuf((__int64)(a1 + 3));
  return a1;
}


==========

FUNCTION: ??0_System_error@std@@IEAA@Verror_code@1@AEBV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@1@@Z @ 0x18000FD8C
----------
__int64 __fastcall std::_System_error::_System_error(__int64 a1, __int128 *a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)&v9 = a1;
  v5 = std::string::string(v10, a3, a3);
  v9 = *a2;
  v6 = std::_System_error::_Makestr((__int64)v11, (__int64)&v9, (__int64)v5);
  *(_QWORD *)&v9 = std::_String_val<std::_Simple_types<char>>::_Myptr(v6);
  *(_QWORD *)a1 = &std::exception::`vftable';
  BYTE8(v9) = 1;
  *(_DWORD *)((char *)&v9 + 9) = 0;
  *(_WORD *)((char *)&v9 + 13) = 0;
  *(_OWORD *)(a1 + 8) = 0i64;
  HIBYTE(v9) = 0;
  o___std_exception_copy_0(&v9);
  *(_QWORD *)a1 = &std::runtime_error::`vftable';
  std::string::_Tidy_deallocate((__int64)v11);
  v7 = *a2;
  *(_QWORD *)a1 = &std::_System_error::`vftable';
  result = a1;
  *(_OWORD *)(a1 + 24) = v7;
  return result;
}


==========

FUNCTION: ??0_System_error@std@@QEAA@AEBV01@@Z @ 0x18000FE58
----------
std::_System_error *__fastcall std::_System_error::_System_error(std::_System_error *this, const struct std::_System_error *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::exception::exception(this, a2);
  *(_QWORD *)this = &std::_System_error::`vftable';
  result = this;
  *(_OWORD *)((char *)this + 24) = *(_OWORD *)((char *)a2 + 24);
  return result;
}


==========

FUNCTION: ??0runtime_error@std@@QEAA@AEBV01@@Z @ 0x18000FE98
----------
std::runtime_error *__fastcall std::runtime_error::runtime_error(std::runtime_error *this, const struct std::runtime_error *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::exception::exception(this, a2);
  *(_QWORD *)this = &std::runtime_error::`vftable';
  return this;
}


==========

FUNCTION: ??0sentry@?$basic_ostream@_WU?$char_traits@_W@std@@@std@@QEAA@AEAV12@@Z @ 0x18000FEC0
----------
__int64 __fastcall std::wostream::sentry::sentry(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)a1 = a2;
  v4 = std::wios::rdbuf(a2 + *(int *)(*(_QWORD *)a2 + 4i64));
  v5 = 0;
  if ( v4 )
    (*(void (__fastcall **)(__int64))(*(_QWORD *)v4 + 8i64))(v4);
  if ( std::ios_base::good((std::ios_base *)(a2 + *(int *)(*(_QWORD *)a2 + 4i64))) )
  {
    v6 = std::wios::tie(a2 + *(int *)(*(_QWORD *)a2 + 4i64));
    if ( !v6 || v6 == a2 )
    {
      v5 = 1;
    }
    else
    {
      std::wostream::flush(v6);
      v5 = std::ios_base::good((std::ios_base *)(a2 + *(int *)(*(_QWORD *)a2 + 4i64)));
    }
  }
  *(_BYTE *)(a1 + 8) = v5;
  return a1;
}


==========

FUNCTION: ??0system_error@std@@QEAA@AEBV01@@Z @ 0x18000FF90
----------
std::system_error *__fastcall std::system_error::system_error(std::system_error *this, const struct std::system_error *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::exception::exception(this, a2);
  *(_QWORD *)this = &std::_System_error::`vftable';
  v4 = *(_OWORD *)((char *)a2 + 24);
  *(_QWORD *)this = &std::system_error::`vftable';
  result = this;
  *(_OWORD *)((char *)this + 24) = v4;
  return result;
}


==========

FUNCTION: ??0system_error@std@@QEAA@HAEBVerror_category@1@AEBV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@1@@Z @ 0x18000FFD8
----------
_QWORD *__fastcall std::system_error::system_error(_QWORD *a1, int a2, __int64 a3, __int64 a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  LODWORD(v6) = a2;
  *((_QWORD *)&v6 + 1) = &mi::mi_category_var;
  std::_System_error::_System_error((__int64)a1, &v6, a4);
  *a1 = &std::system_error::`vftable';
  return a1;
}


==========

FUNCTION: ??1?$function@$$A6A?AW4Type@Response@mi@@AEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@W4_MI_PromptType@@@Z@std@@QEAA@XZ @ 0x180010024
----------
// attributes: thunk
__int64 __fastcall std::function<enum mi::Response::Type (std::wstring const &,enum _MI_PromptType)>::~function<enum mi::Response::Type (std::wstring const &,enum _MI_PromptType)>(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return std::function<void (CLUSPROP_ULARGE_INTEGER *)>::~function<void (CLUSPROP_ULARGE_INTEGER *)>(a1, a2);
}


==========

FUNCTION: ??1?$_Tidy_deallocate_guard@V?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@@std@@QEAA@XZ @ 0x180010030
----------
void __fastcall std::_Tidy_deallocate_guard<std::string>::~_Tidy_deallocate_guard<std::string>(__int64 *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v1 = *a1;
  if ( v1 )
    std::string::_Tidy_deallocate(v1);
}


==========

FUNCTION: ??1?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@QEAA@XZ @ 0x180010050
----------
// attributes: thunk
void __fastcall std::string::~string(std::string *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::string::_Tidy_deallocate((__int64)this);
}


==========

FUNCTION: ??1?$basic_stringbuf@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UEAA@XZ @ 0x18001005C
----------
__int64 __fastcall std::wstringbuf::~wstringbuf(std::wstringbuf *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)this = &std::wstringbuf::`vftable';
  std::wstringbuf::_Tidy((__int64)this);
  return std::wstreambuf::~wstreambuf<wchar_t,std::char_traits<wchar_t>>(this);
}


==========

FUNCTION: ??1?$basic_stringstream@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UEAA@XZ @ 0x180010090
----------
__int64 __fastcall std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::~basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)(*(int *)(*(_QWORD *)(a1 - 152) + 4i64) + a1 - 152) = &std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::`vftable';
  v2 = *(int *)(*(_QWORD *)(a1 - 152) + 4i64);
  *(_DWORD *)(v2 + a1 - 156) = v2 - 152;
  std::wstringbuf::~wstringbuf((std::wstringbuf *)(a1 - 128));
  return std::wiostream::~basic_iostream<wchar_t,std::char_traits<wchar_t>>(a1 - 120);
}


==========

FUNCTION: ??1?$vector@VMiInstance@mi@@V?$allocator@VMiInstance@mi@@@std@@@std@@QEAA@XZ @ 0x1800100F4
----------
void __fastcall std::vector<mi::MiInstance>::~vector<mi::MiInstance>(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(mi::MiInstance **)a1;
  if ( v2 )
  {
    std::_Destroy_range<std::allocator<mi::MiInstance>>(v2, *(mi::MiInstance **)(a1 + 8));
    std::_Deallocate<16,0>(*(void **)a1, 8 * ((__int64)(*(_QWORD *)(a1 + 16) - *(_QWORD *)a1) /*signed*/>> 3));
    *(_QWORD *)a1 = 0i64;
    *(_QWORD *)(a1 + 8) = 0i64;
    *(_QWORD *)(a1 + 16) = 0i64;
  }
}


==========

FUNCTION: ??1_Sentry_base@?$basic_ostream@_WU?$char_traits@_W@std@@@std@@QEAA@XZ @ 0x18001020C
----------
__int64 __fastcall std::wostream::_Sentry_base::~_Sentry_base(std::wostream::_Sentry_base *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  result = std::wios::rdbuf(*(_QWORD *)this + *(int *)(**(_QWORD **)this + 4i64));
  if ( result )
    result = (*(__int64 (__fastcall **)(__int64))(*(_QWORD *)result + 16i64))(result);
  return result;
}


==========

FUNCTION: ??1sentry@?$basic_ostream@_WU?$char_traits@_W@std@@@std@@QEAA@XZ @ 0x180010250
----------
__int64 __fastcall std::wostream::sentry::~sentry(std::wostream::sentry *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !std::uncaught_exception() )
    std::wostream::_Osfx(*(_QWORD *)this);
  return std::wostream::_Sentry_base::~_Sentry_base(this);
}


==========

FUNCTION: ??_D?$basic_stringstream@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@QEAAXXZ @ 0x18001056C
----------
__int64 __fastcall std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::`vbase destructor(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v1 = a1 + 152;
  std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::~basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>(a1 + 152);
  return std::wios::~wios<wchar_t,std::char_traits<wchar_t>>(v1);
}


==========

FUNCTION: ??_G?$basic_stringbuf@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UEAAPEAXI@Z @ 0x1800105C0
----------
void *__fastcall std::wstringbuf::`scalar deleting destructor'(void *Block, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::wstringbuf::~wstringbuf((std::wstringbuf *)Block);
  if ( (a2 & 1) != 0 )
    operator delete(Block);
  return Block;
}


==========

FUNCTION: ??_G?$basic_stringstream@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UEAAPEAXI@Z @ 0x1800105FC
----------
void *__fastcall std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::`scalar deleting destructor'(__int64 a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = (void *)(a1 - 152);
  std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::`vbase destructor(a1 - 152);
  if ( (a2 & 1) != 0 )
    operator delete(v2);
  return v2;
}


==========

FUNCTION: ??_G_System_error@std@@UEAAPEAXI@Z @ 0x180010640
----------
std::_System_error *__fastcall std::_System_error::`scalar deleting destructor'(std::_System_error *this, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)this = &std::exception::`vftable';
  o___std_exception_destroy_0();
  if ( (a2 & 1) != 0 )
    operator delete(this);
  return this;
}


==========

FUNCTION: ?_Calculate_growth@?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@AEBA_K_K@Z @ 0x1800114AC
----------
__int64 __fastcall std::string::_Calculate_growth(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(_QWORD *)(a1 + 24);
  v3 = a2 | 0xF;
  v4 = 0x7FFFFFFFFFFFFFFFi64;
  if ( (a2 | 0xFui64) > 0x7FFFFFFFFFFFFFFFi64 )
    return v4;
  v5 = v2 >> 1;
  if ( v2 > 0x7FFFFFFFFFFFFFFFi64 - (v2 >> 1) )
    return v4;
  v4 = v5 + v2;
  if ( v3 >= v5 + v2 )
    v4 = v3;
  return v4;
}


==========

FUNCTION: ?_Copy@?$_Func_impl_no_alloc@UResultsFunctor@ServerManager@@XAEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEBVMiValue@mi@@@std@@EEBAPEAV?$_Func_base@XAEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEBVMiValue@mi@@@2@PEAX@Z @ 0x1800114F0
----------
_QWORD *__fastcall std::_Func_impl_no_alloc<ServerManager::ResultsFunctor,void,std::wstring const &,mi::MiValue const &>::_Copy(__int64 a1, _QWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a2 = &std::_Func_impl_no_alloc<ServerManager::ResultsFunctor,void,std::wstring const &,mi::MiValue const &>::`vftable';
  a2[1] = *(_QWORD *)(a1 + 8);
  return a2;
}


==========

FUNCTION: ?_Deallocate_for_capacity@?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@CAXAEAV?$allocator@D@2@QEAD_K@Z @ 0x180011568
----------
void __fastcall std::string::_Deallocate_for_capacity(__int64 a1, void *a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::_Deallocate<16,0>(a2, a3 + 1);
}


==========

FUNCTION: ?_Delete_this@?$_Func_impl_no_alloc@UResultsFunctor@ServerManager@@XAEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEBVMiValue@mi@@@std@@EEAAX_N@Z @ 0x180011580
----------
void __fastcall std::_Func_impl_no_alloc<ServerManager::ResultsFunctor,void,std::wstring const &,mi::MiValue const &>::_Delete_this(void *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a2 )
    operator delete(a1);
}


==========

FUNCTION: ?_Do_call@?$_Func_impl_no_alloc@UResultsFunctor@ServerManager@@XAEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEBVMiValue@mi@@@std@@EEAAXAEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@2@AEBVMiValue@mi@@@Z @ 0x1800115E0
----------
__int64 __fastcall std::_Func_impl_no_alloc<ServerManager::ResultsFunctor,void,std::wstring const &,mi::MiValue const &>::_Do_call(__int64 a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return ServerManager::ResultsFunctor::operator()((void (__fastcall **)(_QWORD, __int64, _QWORD, _QWORD *))(a1 + 8), a2, a3);
}


==========

FUNCTION: ?_Get_buffer_view@?$basic_stringbuf@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@QEBA?AU_Buffer_view@12@XZ @ 0x1800116A4
----------
__int64 __fastcall std::wstringbuf::_Get_buffer_view(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_OWORD *)a2 = 0i64;
  *(_QWORD *)(a2 + 16) = 0i64;
  if ( (*(_DWORD *)(a1 + 112) & 0x22) != 2 && ((__int64 (*)(void))std::wstreambuf::pptr)() )
  {
    v4 = std::wstreambuf::pbase(a1);
    *(_QWORD *)a2 = v4;
    v5 = v4;
    v6 = std::wstreambuf::pptr(a1);
    if ( v6 < *(_QWORD *)(a1 + 104) )
      v6 = *(_QWORD *)(a1 + 104);
    *(_QWORD *)(a2 + 8) = (__int64)(v6 - v5) /*signed*/>> 1;
    v7 = (std::wstreambuf::epptr(a1) - v5) /*signed*/>> 1;
    goto LABEL_9;
  }
  if ( (*(_BYTE *)(a1 + 112) & 4) != 0 || !std::wstreambuf::gptr(a1) )
    return a2;
  v8 = std::wstreambuf::eback(a1);
  *(_QWORD *)a2 = v8;
  v7 = (std::wstreambuf::egptr(a1) - v8) /*signed*/>> 1;
  *(_QWORD *)(a2 + 8) = v7;
LABEL_9:
  *(_QWORD *)(a2 + 16) = v7;
  return a2;
}


==========

FUNCTION: ?_Large_mode_engaged@?$_String_val@U?$_Simple_types@D@std@@@std@@QEBA_NXZ @ 0x180011794
----------
bool __fastcall std::_String_val<std::_Simple_types<char>>::_Large_mode_engaged(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return *(_QWORD *)(a1 + 24) > 0xFui64;
}


==========

FUNCTION: ?_Makestr@_System_error@std@@CA?AV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@2@Verror_code@2@V32@@Z @ 0x1800117A4
----------
__int64 __fastcall std::_System_error::_Makestr(__int64 a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( *(_QWORD *)(a3 + 16) )
    std::string::append(a3, ": ", 2i64);
  v6 = std::error_code::message(a2, v11);
  v8 = std::_String_val<std::_Simple_types<char>>::_Myptr(v6, v7, *(_QWORD *)(v6 + 16));
  std::string::append(a3, v8, v9);
  std::string::_Tidy_deallocate(v11);
  *(_OWORD *)a1 = 0i64;
  *(_QWORD *)(a1 + 16) = 0i64;
  *(_QWORD *)(a1 + 24) = 0i64;
  *(_OWORD *)a1 = *(_OWORD *)a3;
  *(_OWORD *)(a1 + 16) = *(_OWORD *)(a3 + 16);
  *(_QWORD *)(a3 + 16) = 0i64;
  *(_QWORD *)(a3 + 24) = 15i64;
  *(_BYTE *)a3 = 0;
  std::string::_Tidy_deallocate(a3);
  return a1;
}


==========

FUNCTION: ?_Myptr@?$_String_val@U?$_Simple_types@D@std@@@std@@QEBAPEBDXZ @ 0x180011870
----------
_QWORD *__fastcall std::_String_val<std::_Simple_types<char>>::_Myptr(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( std::_String_val<std::_Simple_types<char>>::_Large_mode_engaged(a1) )
    v1 = (_QWORD *)*v1;
  return v1;
}


==========

FUNCTION: ?_Target_type@?$_Func_impl_no_alloc@UResultsFunctor@ServerManager@@XAEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEBVMiValue@mi@@@std@@EEBAAEBVtype_info@@XZ @ 0x180011890
----------
void *std::_Func_impl_no_alloc<ServerManager::ResultsFunctor,void,std::wstring const &,mi::MiValue const &>::_Target_type()
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return &ServerManager::ResultsFunctor `RTTI Type Descriptor';
}


==========

FUNCTION: ?_Tidy@?$basic_stringbuf@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@IEAAXXZ @ 0x1800118C0
----------
__int64 __fastcall std::wstringbuf::_Tidy(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( (*(_BYTE *)(a1 + 112) & 1) != 0 )
  {
    if ( std::wstreambuf::pptr(a1) )
      v2 = std::wstreambuf::epptr(a1);
    else
      v2 = std::wstreambuf::egptr(a1);
    v3 = (v2 - std::wstreambuf::eback(a1)) /*signed*/>> 1;
    v4 = (void *)std::wstreambuf::eback(a1);
    std::_Deallocate<16,0>(v4, 2 * v3);
  }
  std::wstreambuf::setg(a1, 0i64, 0i64, 0i64);
  result = std::wstreambuf::setp(a1, 0i64, 0i64);
  *(_QWORD *)(a1 + 104) = 0i64;
  *(_DWORD *)(a1 + 112) &= 0xFFFFFFFE;
  return result;
}


==========

FUNCTION: ?_Tidy_deallocate@?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@AEAAXXZ @ 0x18001197C
----------
void __fastcall std::string::_Tidy_deallocate(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( std::_String_val<std::_Simple_types<char>>::_Large_mode_engaged(a1) )
    std::string::_Deallocate_for_capacity(v2, *(void **)v2, *(_QWORD *)(v2 + 24));
  *(_QWORD *)(a1 + 16) = 0i64;
  *(_QWORD *)(a1 + 24) = 15i64;
  *(_BYTE *)a1 = 0;
}


==========

FUNCTION: ?append@?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@QEAAAEAV12@QEBD_K@Z @ 0x1800119C0
----------
void *__fastcall std::string::append(_QWORD *a1, const void *a2, unsigned __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = a1[2];
  if ( a3 > a1[3] - v4 )
    return std::string::_Reallocate_grow_by<_lambda_65e615be2a453ca0576c979606f46740_,char const *,unsigned __int64>(a1, a3, v4, a2, a3);
  a1[2] = v4 + a3;
  v6 = std::_String_val<std::_Simple_types<char>>::_Myptr((__int64)a1);
  v8 = (char *)v6 + v7;
  memmove_0((char *)v6 + v7, v9, a3);
  result = a1;
  v8[a3] = 0;
  return result;
}


==========

FUNCTION: ?assign@?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@QEAAAEAV12@QEB_W_K@Z @ 0x180011A34
----------
__int64 __fastcall std::wstring::assign(__int64 a1, const void *a2, unsigned __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a3 > *(_QWORD *)(a1 + 24) )
    return std::wstring::_Reallocate_for<_lambda_3fa8b2c8193a0f3144fc4b1b8f243931_,wchar_t const *>(a1, a3, a3, a2);
  v4 = std::_String_val<std::_Simple_types<wchar_t>>::_Myptr(a1);
  v6 = 2 * v5;
  *(_QWORD *)(a1 + 16) = v5;
  v7 = v4;
  memmove_0(v4, v8, 2 * v5);
  *(_WORD *)((char *)v7 + v6) = 0;
  return a1;
}


==========

FUNCTION: ?clear@?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@QEAAXXZ @ 0x180011BE4
----------
__int64 __fastcall std::vector<std::wstring>::clear(__int64 *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v1 = a1[1];
  if ( *a1 == v1 )
    return result;
  std::_Destroy_range<std::allocator<std::wstring>>(*a1, v1);
  result = *a1;
  a1[1] = *a1;
  return result;
}


==========

FUNCTION: ?eq_int_type@?$_WChar_traits@_W@std@@SA_NGG@Z @ 0x180011C14
----------
bool __fastcall std::_WChar_traits<wchar_t>::eq_int_type(__int16 a1, __int16 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return a1 == a2;
}


==========

FUNCTION: ?message@error_code@std@@QEBA?AV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@2@XZ @ 0x180011ED4
----------
__int64 __fastcall std::error_code::message(unsigned int *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  (*(void (__fastcall **)(_QWORD, __int64, _QWORD))(**((_QWORD **)a1 + 1) + 16i64))(*((_QWORD *)a1 + 1), a2, *a1);
  return a2;
}


==========

FUNCTION: ?not_eof@?$_WChar_traits@_W@std@@SAGG@Z @ 0x180011F00
----------
__int64 __fastcall std::_WChar_traits<wchar_t>::not_eof(unsigned __int16 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a1 == 0xFFFF )
    a1 = 0;
  return a1;
}


==========

FUNCTION: ?overflow@?$basic_stringbuf@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@MEAAGG@Z @ 0x180011F20
----------
__int64 __fastcall std::wstringbuf::overflow(__int64 a1, unsigned __int16 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( (*(_BYTE *)(a1 + 112) & 2) != 0 )
    return 0xFFFFi64;
  v4 = 0i64;
  if ( std::_WChar_traits<wchar_t>::eq_int_type(0xFFFF, a2) )
    return std::_WChar_traits<wchar_t>::not_eof(a2);
  v6 = std::wstreambuf::pptr(a1);
  v7 = std::wstreambuf::epptr(a1);
  v8 = v7;
  if ( v6 && v6 < v7 )
  {
    *(_WORD *)std::wstreambuf::_Pninc(a1) = a2;
    *(_QWORD *)(a1 + 104) = v6 + 2;
    return a2;
  }
  v9 = std::wstreambuf::eback(a1);
  v11 = 32i64;
  v12 = (void *)v9;
  if ( !v6 || (v4 = (__int64)(v8 - v9) /*signed*/>> 1, v4 < 0x20) )
  {
LABEL_13:
    v20 = v11;
    v13 = 2 * v4;
    v14 = std::_Allocate_at_least_helper<std::allocator<wchar_t>>(v10, &v20);
    memcpy_0(v14, v12, 2 * v4);
    *(_QWORD *)(a1 + 104) = (char *)v14 + 2 * v4 + 2;
    std::wstreambuf::setp(a1, v14, (char *)v14 + 2 * v4, (char *)v14 + 2 * v11);
    v15 = a1;
    if ( (*(_BYTE *)(a1 + 112) & 4) != 0 )
    {
      v16 = v14;
      v17 = (char *)v14;
    }
    else
    {
      v18 = *(_QWORD **)(a1 + 104);
      v19 = std::wstreambuf::gptr(a1);
      v16 = v18;
      v15 = a1;
      v17 = (char *)v14 + 2 * ((v19 - (__int64)v12) /*signed*/>> 1);
    }
    std::wstreambuf::setg(v15, v14, v17, v16);
    if ( (*(_BYTE *)(a1 + 112) & 1) != 0 )
      std::_Deallocate<16,0>(v12, v13);
    *(_DWORD *)(a1 + 112) |= 1u;
    *(_WORD *)std::wstreambuf::_Pninc(a1) = a2;
    return a2;
  }
  if ( v4 < 0x3FFFFFFF )
  {
    v11 = 2 * v4;
    goto LABEL_13;
  }
  v11 = 0x7FFFFFFFi64;
  if ( v4 < 0x7FFFFFFF )
    goto LABEL_13;
  return 0xFFFFi64;
}


==========

FUNCTION: ?pbackfail@?$basic_stringbuf@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@MEAAGG@Z @ 0x1800120E0
----------
__int64 __fastcall std::wstringbuf::pbackfail(__int64 a1, __int16 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = std::wstreambuf::gptr(a1);
  if ( !v4 )
    return 0xFFFFi64;
  if ( v4 <= std::wstreambuf::eback(a1) )
    return 0xFFFFi64;
  v5 = std::_WChar_traits<wchar_t>::eq_int_type(0xFFFF, a2);
  if ( !v5 && !std::_WChar_traits<wchar_t>::eq_int_type(a2, *(_WORD *)(v4 - 2)) && (*(_BYTE *)(a1 + 112) & 2) != 0 )
    return 0xFFFFi64;
  std::wstreambuf::gbump(a1, 0xFFFFFFFFi64);
  if ( !v5 )
    *(_WORD *)std::wstreambuf::gptr(a1) = a2;
  return std::_WChar_traits<wchar_t>::not_eof(a2);
}


==========

FUNCTION: ?seekoff@?$basic_stringbuf@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@MEAA?AV?$fpos@U_Mbstatet@@@2@_JHH@Z @ 0x1800121B0
----------
_QWORD *__fastcall std::wstringbuf::seekoff(__int64 a1, _QWORD *a2, __int64 a3, int a4, char a5)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v6 = (_BYTE *)(a1 + 112);
  v9 = (a5 & 1) != 0 && (*v6 & 4) != 0;
  v10 = (a5 & 2) != 0 && (*v6 & 2) != 0;
  if ( v9 || v10 )
    goto LABEL_44;
  v11 = **(_QWORD **)(a1 + 56);
  if ( (*v6 & 2) != 0 )
  {
    v12 = 0i64;
  }
  else
  {
    v12 = **(_QWORD **)(a1 + 64);
    if ( v12 && *(_QWORD *)(a1 + 104) < v12 )
      *(_QWORD *)(a1 + 104) = v12;
  }
  v13 = *(__int64 **)(a1 + 24);
  v14 = *v13;
  v15 = (*(_QWORD *)(a1 + 104) - *v13) /*signed*/>> 1;
  if ( !a4 )
  {
    v17 = 0i64;
    goto LABEL_31;
  }
  v16 = a4 - 1;
  if ( !v16 )
  {
    if ( (a5 & 3) != 3 )
    {
      if ( (a5 & 1) != 0 )
      {
        if ( !v11 && v14 )
          goto LABEL_44;
        v18 = v11;
      }
      else
      {
        if ( (a5 & 2) == 0 || !v12 && v14 )
          goto LABEL_44;
        v18 = v12;
      }
      v17 = (__int64)(v18 - v14) /*signed*/>> 1;
      goto LABEL_31;
    }
LABEL_44:
    *a2 = -1i64;
    goto LABEL_45;
  }
  if ( v16 != 1 )
    goto LABEL_44;
  v17 = (*(_QWORD *)(a1 + 104) - *v13) /*signed*/>> 1;
LABEL_31:
  v19 = v17 + a3;
  if ( v17 + a3 > v15 || v19 && ((a5 & 1) != 0 && !v11 || (a5 & 2) != 0 && !v12) )
    goto LABEL_44;
  v20 = v14 + 2 * v19;
  if ( (a5 & 1) != 0 && v11 )
  {
    v21 = (*(_QWORD *)(a1 + 104) - v20) /*signed*/>> 1;
    **(_QWORD **)(a1 + 24) = v14;
    **(_QWORD **)(a1 + 56) = v20;
    **(_DWORD **)(a1 + 80) = v21;
  }
  if ( (a5 & 2) != 0 && v12 )
  {
    v22 = **(int **)(a1 + 88);
    v23 = **(_QWORD **)(a1 + 64);
    **(_QWORD **)(a1 + 32) = v14;
    **(_QWORD **)(a1 + 64) = v20;
    **(_DWORD **)(a1 + 88) = (v23 + 2 * v22 - v20) /*signed*/>> 1;
  }
  *a2 = v19;
LABEL_45:
  a2[1] = 0i64;
  a2[2] = 0i64;
  return a2;
}


==========

FUNCTION: ?seekpos@?$basic_stringbuf@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@MEAA?AV?$fpos@U_Mbstatet@@@2@V32@H@Z @ 0x180012360
----------
unsigned __int64 *__fastcall std::wstringbuf::seekpos(__int64 a1, unsigned __int64 *a2, _QWORD *a3, char a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v5 = (_BYTE *)(a1 + 112);
  v8 = (a4 & 1) != 0 && (*v5 & 4) != 0;
  v9 = (a4 & 2) != 0 && (*v5 & 2) != 0;
  if ( v8 || v9 )
    goto LABEL_29;
  v10 = *a3 + a3[1];
  v11 = **(_QWORD **)(a1 + 56);
  if ( (*v5 & 2) != 0 )
  {
    v12 = 0i64;
  }
  else
  {
    v12 = **(_QWORD **)(a1 + 64);
    if ( v12 && *(_QWORD *)(a1 + 104) < v12 )
      *(_QWORD *)(a1 + 104) = v12;
  }
  v13 = *(__int64 **)(a1 + 24);
  v14 = *(_QWORD *)(a1 + 104);
  v15 = *v13;
  if ( v10 > (v14 - *v13) /*signed*/>> 1 || v10 && ((a4 & 1) != 0 && !v11 || (a4 & 2) != 0 && !v12) )
  {
LABEL_29:
    *a2 = -1i64;
  }
  else
  {
    v16 = v15 + 2 * v10;
    if ( (a4 & 1) != 0 && v11 )
    {
      *v13 = v15;
      **(_QWORD **)(a1 + 56) = v16;
      **(_DWORD **)(a1 + 80) = (v14 - v16) /*signed*/>> 1;
    }
    if ( (a4 & 2) != 0 && v12 )
    {
      v17 = **(int **)(a1 + 88);
      v18 = **(_QWORD **)(a1 + 64);
      **(_QWORD **)(a1 + 32) = v15;
      **(_QWORD **)(a1 + 64) = v16;
      **(_DWORD **)(a1 + 88) = (v18 + 2 * v17 - v16) /*signed*/>> 1;
    }
    *a2 = v10;
  }
  a2[1] = 0i64;
  a2[2] = 0i64;
  return a2;
}


==========

FUNCTION: ?str@?$basic_stringbuf@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@QEBA?AV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@2@XZ @ 0x1800124A0
----------
__int64 __fastcall std::wstringbuf::str(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_OWORD *)a2 = 0i64;
  *(_QWORD *)(a2 + 16) = 0i64;
  *(_QWORD *)(a2 + 24) = 7i64;
  *(_WORD *)a2 = 0;
  std::wstringbuf::_Get_buffer_view(a1, (__int64)v4);
  if ( v4[0] )
    std::wstring::assign(a2, v4[0], (unsigned __int64)v4[1]);
  return a2;
}


==========

FUNCTION: ?str@?$basic_stringstream@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@QEBA?AV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@2@XZ @ 0x18001250C
----------
__int64 __fastcall std::basic_stringstream<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t>>::str(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::wstringbuf::str(a1 + 24, a2);
  return a2;
}


==========

FUNCTION: ?underflow@?$basic_stringbuf@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@MEAAGXZ @ 0x180012530
----------
__int64 __fastcall std::wstringbuf::underflow(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = (unsigned __int16 *)std::wstreambuf::gptr(a1);
  if ( !v2 )
    return 0xFFFFi64;
  if ( (unsigned __int64)v2 < std::wstreambuf::egptr(a1) )
    return *v2;
  v4 = std::wstreambuf::pptr(a1);
  if ( !v4 || (*(_BYTE *)(a1 + 112) & 4) != 0 )
    return 0xFFFFi64;
  v5 = *(_QWORD *)(a1 + 104);
  if ( v5 < v4 )
    v5 = v4;
  if ( v5 <= (unsigned __int64)v2 )
    return 0xFFFFi64;
  *(_QWORD *)(a1 + 104) = v5;
  v6 = std::wstreambuf::gptr(a1);
  v7 = std::wstreambuf::eback(a1);
  std::wstreambuf::setg(a1, v7, v6, v5);
  return *(unsigned __int16 *)std::wstreambuf::gptr(a1);
}


==========

FUNCTION: ??$?8$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@$$CBV01@U234@@std@@YA_NAEBU?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@0@0@Z @ 0x180012610
----------
char __fastcall std::operator==<std::wstring const,cxl::PropertyList::Property,std::wstring const,cxl::PropertyList::Property>(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( *(_QWORD *)(a1 + 16) != *(_QWORD *)(a2 + 16) )
    return 0;
  std::_String_val<std::_Simple_types<wchar_t>>::_Myptr(a2);
  v3 = (char *)std::_String_val<std::_Simple_types<wchar_t>>::_Myptr(v2);
  if ( v5 )
  {
    v8 = v3 - v6;
    while ( *(_WORD *)&v6[v8] == *(_WORD *)v6 )
    {
      v6 += 2;
      if ( !--v5 )
        goto LABEL_6;
    }
    return 0;
  }
LABEL_6:
  if ( !(unsigned __int8)cxl::PropertyList::Property::operator==(v7 + 32, v4 + 32, v5, v6) )
    return 0;
  return v9;
}


==========

FUNCTION: ??$?8EV?$allocator@E@std@@@std@@YA_NAEBV?$vector@EV?$allocator@E@std@@@0@0@Z @ 0x18001267C
----------
bool __fastcall std::operator==<unsigned char,std::allocator<unsigned char>>(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(_QWORD *)(a1 + 8) - *(_QWORD *)a1;
  if ( v2 == *(_QWORD *)(a2 + 8) - *(_QWORD *)a2 )
    result = memcmp_0(*(const void **)a1, *(const void **)a2, v2) == 0;
  else
    result = 0;
  return result;
}


==========

FUNCTION: ??$_Allocate_at_least_helper@V?$allocator@U?$pair@_K_K@std@@@std@@@std@@YAPEAU?$pair@_K_K@0@AEAV?$allocator@U?$pair@_K_K@std@@@0@AEA_K@Z @ 0x180014320
----------
_QWORD *__fastcall std::_Allocate_at_least_helper<std::allocator<std::pair<unsigned __int64,unsigned __int64>>>(__int64 a1, _QWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( *a2 > 0xFFFFFFFFFFFFFFFui64 )
    std::_Throw_bad_array_new_length();
  return std::_Allocate<16,std::_Default_allocate_traits,0>(16i64 * *a2);
}


==========

FUNCTION: ??$_Buyheadnode@V?$allocator@U?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@std@@@std@@@?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@std@@SAPEAU01@AEAV?$allocator@U?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@std@@@1@@Z @ 0x180014350
----------
_QWORD *std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>::_Buyheadnode<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>()
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  result = operator new(0x50ui64);
  *result = result;
  result[1] = result;
  result[2] = result;
  *((_WORD *)result + 12) = 257;
  return result;
}


==========

FUNCTION: ??$_Construct_in_place@U?$pair@_K_K@std@@U12@@std@@YAXAEAU?$pair@_K_K@0@$$QEAU10@@Z @ 0x18001437C
----------
void __fastcall std::_Construct_in_place<std::pair<unsigned __int64,unsigned __int64>,std::pair<unsigned __int64,unsigned __int64>>(_OWORD *a1, _OWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = *a2;
}


==========

FUNCTION: ??$_Construct_in_place@V?$ValueElement@GUCLUSPROP_WORD@@@ValueList@cxl@@PEAUCLUSPROP_WORD@@AEAVlist_accessor@23@@std@@YAXAEAV?$ValueElement@GUCLUSPROP_WORD@@@ValueList@cxl@@$$QEAPEAUCLUSPROP_WORD@@AEAVlist_accessor@23@@Z @ 0x18001438C
----------
void **__fastcall std::_Construct_in_place<cxl::ValueList::ValueElement<unsigned short,CLUSPROP_WORD>,CLUSPROP_WORD *,cxl::ValueList::list_accessor &>(_QWORD *a1, __int64 *a2, _QWORD *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = *a2;
  *a1 = &cxl::ValueList::ValueItem<unsigned short>::`vftable';
  a1[1] = v3;
  a1[2] = *a3;
  a1[3] = a3[1];
  result = &cxl::ValueList::ValueElement<unsigned short,CLUSPROP_WORD>::`vftable';
  *a1 = &cxl::ValueList::ValueElement<unsigned short,CLUSPROP_WORD>::`vftable';
  a1[5] = v3;
  return result;
}


==========

FUNCTION: ??$_Construct_in_place@V?$ValueElement@JUCLUSPROP_LONG@@@ValueList@cxl@@PEAUCLUSPROP_LONG@@AEAVlist_accessor@23@@std@@YAXAEAV?$ValueElement@JUCLUSPROP_LONG@@@ValueList@cxl@@$$QEAPEAUCLUSPROP_LONG@@AEAVlist_accessor@23@@Z @ 0x1800143C4
----------
void **__fastcall std::_Construct_in_place<cxl::ValueList::ValueElement<long,CLUSPROP_LONG>,CLUSPROP_LONG *,cxl::ValueList::list_accessor &>(_QWORD *a1, __int64 *a2, _QWORD *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = *a2;
  *a1 = &cxl::ValueList::ValueItem<long>::`vftable';
  a1[1] = v3;
  a1[2] = *a3;
  a1[3] = a3[1];
  result = &cxl::ValueList::ValueElement<long,CLUSPROP_LONG>::`vftable';
  *a1 = &cxl::ValueList::ValueElement<long,CLUSPROP_LONG>::`vftable';
  a1[5] = v3;
  return result;
}


==========

FUNCTION: ??$_Construct_in_place@V?$ValueElement@KUCLUSPROP_DWORD@@@ValueList@cxl@@PEAUCLUSPROP_DWORD@@AEAVlist_accessor@23@@std@@YAXAEAV?$ValueElement@KUCLUSPROP_DWORD@@@ValueList@cxl@@$$QEAPEAUCLUSPROP_DWORD@@AEAVlist_accessor@23@@Z @ 0x1800143FC
----------
void **__fastcall std::_Construct_in_place<cxl::ValueList::ValueElement<unsigned long,CLUSPROP_DWORD>,CLUSPROP_DWORD *,cxl::ValueList::list_accessor &>(_QWORD *a1, __int64 *a2, _QWORD *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = *a2;
  *a1 = &cxl::ValueList::ValueItem<unsigned long>::`vftable';
  a1[1] = v3;
  a1[2] = *a3;
  a1[3] = a3[1];
  result = &cxl::ValueList::ValueElement<unsigned long,CLUSPROP_DWORD>::`vftable';
  *a1 = &cxl::ValueList::ValueElement<unsigned long,CLUSPROP_DWORD>::`vftable';
  a1[5] = v3;
  return result;
}


==========

FUNCTION: ??$_Construct_in_place@V?$ValueElement@T_LARGE_INTEGER@@UCLUSPROP_LARGE_INTEGER@@@ValueList@cxl@@PEAUCLUSPROP_LARGE_INTEGER@@AEAVlist_accessor@23@@std@@YAXAEAV?$ValueElement@T_LARGE_INTEGER@@UCLUSPROP_LARGE_INTEGER@@@ValueList@cxl@@$$QEAPEAUCLUSPROP_LARGE_INTEGER@@AEAVlist_accessor@23@@Z @ 0x180014434
----------
void **__fastcall std::_Construct_in_place<cxl::ValueList::ValueElement<_LARGE_INTEGER,CLUSPROP_LARGE_INTEGER>,CLUSPROP_LARGE_INTEGER *,cxl::ValueList::list_accessor &>(_QWORD *a1, __int64 *a2, _QWORD *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = *a2;
  *a1 = &cxl::ValueList::ValueItem<_LARGE_INTEGER>::`vftable';
  a1[1] = v3;
  a1[2] = *a3;
  a1[3] = a3[1];
  result = &cxl::ValueList::ValueElement<_LARGE_INTEGER,CLUSPROP_LARGE_INTEGER>::`vftable';
  *a1 = &cxl::ValueList::ValueElement<_LARGE_INTEGER,CLUSPROP_LARGE_INTEGER>::`vftable';
  a1[5] = v3;
  return result;
}


==========

FUNCTION: ??$_Construct_in_place@V?$ValueElement@T_ULARGE_INTEGER@@UCLUSPROP_ULARGE_INTEGER@@@ValueList@cxl@@PEAUCLUSPROP_ULARGE_INTEGER@@AEAVlist_accessor@23@@std@@YAXAEAV?$ValueElement@T_ULARGE_INTEGER@@UCLUSPROP_ULARGE_INTEGER@@@ValueList@cxl@@$$QEAPEAUCLUSPROP_ULARGE_INTEGER@@AEAVlist_accessor@23@@Z @ 0x18001446C
----------
void **__fastcall std::_Construct_in_place<cxl::ValueList::ValueElement<_ULARGE_INTEGER,CLUSPROP_ULARGE_INTEGER>,CLUSPROP_ULARGE_INTEGER *,cxl::ValueList::list_accessor &>(_QWORD *a1, __int64 *a2, _QWORD *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = *a2;
  *a1 = &cxl::ValueList::ValueItem<_ULARGE_INTEGER>::`vftable';
  a1[1] = v3;
  a1[2] = *a3;
  a1[3] = a3[1];
  result = &cxl::ValueList::ValueElement<_ULARGE_INTEGER,CLUSPROP_ULARGE_INTEGER>::`vftable';
  *a1 = &cxl::ValueList::ValueElement<_ULARGE_INTEGER,CLUSPROP_ULARGE_INTEGER>::`vftable';
  a1[5] = v3;
  return result;
}


==========

FUNCTION: ??$_Construct_in_place@V?$ValueElement@U_FILETIME@@UCLUSPROP_FILETIME@@@ValueList@cxl@@PEAUCLUSPROP_FILETIME@@AEAVlist_accessor@23@@std@@YAXAEAV?$ValueElement@U_FILETIME@@UCLUSPROP_FILETIME@@@ValueList@cxl@@$$QEAPEAUCLUSPROP_FILETIME@@AEAVlist_accessor@23@@Z @ 0x1800144A4
----------
void **__fastcall std::_Construct_in_place<cxl::ValueList::ValueElement<_FILETIME,CLUSPROP_FILETIME>,CLUSPROP_FILETIME *,cxl::ValueList::list_accessor &>(_QWORD *a1, __int64 *a2, _QWORD *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = *a2;
  *a1 = &cxl::ValueList::ValueItem<_FILETIME>::`vftable';
  a1[1] = v3;
  a1[2] = *a3;
  a1[3] = a3[1];
  result = &cxl::ValueList::ValueElement<_FILETIME,CLUSPROP_FILETIME>::`vftable';
  *a1 = &cxl::ValueList::ValueElement<_FILETIME,CLUSPROP_FILETIME>::`vftable';
  a1[5] = v3;
  return result;
}


==========

FUNCTION: ??$_Construct_in_place@V?$ValueElement@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UCLUSPROP_SZ@@@ValueList@cxl@@PEAUCLUSPROP_SZ@@AEAVlist_accessor@23@@std@@YAXAEAV?$ValueElement@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UCLUSPROP_SZ@@@ValueList@cxl@@$$QEAPEAUCLUSPROP_SZ@@AEAVlist_accessor@23@@Z @ 0x1800144DC
----------
void **__fastcall std::_Construct_in_place<cxl::ValueList::ValueElement<std::wstring,CLUSPROP_SZ>,CLUSPROP_SZ *,cxl::ValueList::list_accessor &>(_QWORD *a1, __int64 *a2, _QWORD *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = *a2;
  *a1 = &cxl::ValueList::ValueItem<std::wstring>::`vftable';
  a1[1] = v3;
  a1[2] = *a3;
  a1[3] = a3[1];
  result = &cxl::ValueList::ValueElement<std::wstring,CLUSPROP_SZ>::`vftable';
  *a1 = &cxl::ValueList::ValueElement<std::wstring,CLUSPROP_SZ>::`vftable';
  a1[5] = v3;
  return result;
}


==========

FUNCTION: ??$_Construct_in_place@V?$ValueElement@V?$vector@EV?$allocator@E@std@@@std@@UCLUSPROP_BINARY@@@ValueList@cxl@@PEAUCLUSPROP_BINARY@@AEAVlist_accessor@23@@std@@YAXAEAV?$ValueElement@V?$vector@EV?$allocator@E@std@@@std@@UCLUSPROP_BINARY@@@ValueList@cxl@@$$QEAPEAUCLUSPROP_BINARY@@AEAVlist_accessor@23@@Z @ 0x180014514
----------
void **__fastcall std::_Construct_in_place<cxl::ValueList::ValueElement<std::vector<unsigned char>,CLUSPROP_BINARY>,CLUSPROP_BINARY *,cxl::ValueList::list_accessor &>(_QWORD *a1, __int64 *a2, _QWORD *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = *a2;
  *a1 = &cxl::ValueList::ValueItem<std::vector<unsigned char>>::`vftable';
  a1[1] = v3;
  a1[2] = *a3;
  a1[3] = a3[1];
  result = &cxl::ValueList::ValueElement<std::vector<unsigned char>,CLUSPROP_BINARY>::`vftable';
  *a1 = &cxl::ValueList::ValueElement<std::vector<unsigned char>,CLUSPROP_BINARY>::`vftable';
  a1[5] = v3;
  return result;
}


==========

FUNCTION: ??$_Construct_in_place@V?$ValueElement@V?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@UCLUSPROP_SZ@@@ValueList@cxl@@PEAUCLUSPROP_SZ@@AEAVlist_accessor@23@@std@@YAXAEAV?$ValueElement@V?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@UCLUSPROP_SZ@@@ValueList@cxl@@$$QEAPEAUCLUSPROP_SZ@@AEAVlist_accessor@23@@Z @ 0x18001454C
----------
void **__fastcall std::_Construct_in_place<cxl::ValueList::ValueElement<std::vector<std::wstring>,CLUSPROP_SZ>,CLUSPROP_SZ *,cxl::ValueList::list_accessor &>(_QWORD *a1, __int64 *a2, _QWORD *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = *a2;
  *a1 = &cxl::ValueList::ValueItem<std::vector<std::wstring>>::`vftable';
  a1[1] = v3;
  a1[2] = *a3;
  a1[3] = a3[1];
  result = &cxl::ValueList::ValueElement<std::vector<std::wstring>,CLUSPROP_SZ>::`vftable';
  *a1 = &cxl::ValueList::ValueElement<std::vector<std::wstring>,CLUSPROP_SZ>::`vftable';
  a1[5] = v3;
  return result;
}


==========

FUNCTION: ??$_Construct_in_place@V?$tuple@_KUProperty@PropertyList@cxl@@_N@std@@V12@@std@@YAXAEAV?$tuple@_KUProperty@PropertyList@cxl@@_N@0@$$QEAV10@@Z @ 0x180014584
----------
__int64 __fastcall std::_Construct_in_place<std::tuple<unsigned __int64,cxl::PropertyList::Property,bool>,std::tuple<unsigned __int64,cxl::PropertyList::Property,bool>>(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_BYTE *)a1 = *(_BYTE *)a2;
  *(_OWORD *)(a1 + 8) = *(_OWORD *)(a2 + 8);
  result = *(_QWORD *)(a2 + 24);
  *(_QWORD *)(a1 + 24) = result;
  return result;
}


==========

FUNCTION: ??$_Emplace@U?$pair@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@@?$_Tree@V?$_Tmap_traits@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@UCaseInsensitive_LessThan@5@V?$allocator@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@@2@$0A@@std@@@std@@IEAA?AU?$pair@PEAU?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@std@@_N@1@$$QEAU?$pair@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@1@@Z @ 0x1800145A4
----------
__int64 __fastcall std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::_Emplace<std::pair<std::wstring,cxl::PropertyList::Property>>(__int64 *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v6 = std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::_Find_lower_bound<std::wstring>(a1, &v14);
  v7 = *(_OWORD *)v6;
  v15 = *(_QWORD *)(v6 + 16);
  v8 = v15;
  if ( (unsigned __int8)std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::_Lower_bound_duplicate<std::wstring>(v9, v15, a3) )
  {
    *(_QWORD *)a2 = v8;
    *(_BYTE *)(a2 + 8) = 0;
  }
  else
  {
    if ( a1[1] == 0x333333333333333i64 )
      std::_Xlength_error("map/set too long");
    v10 = *a1;
    v14 = (unsigned __int64)a1;
    v11 = operator new(0x50ui64);
    std::_Default_allocator_traits<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>::construct<std::pair<std::wstring const,cxl::PropertyList::Property>,std::pair<std::wstring,cxl::PropertyList::Property>>(v12, v11 + 4, a3);
    *v11 = v10;
    v11[1] = v10;
    v11[2] = v10;
    *((_WORD *)v11 + 12) = 0;
    *((_QWORD *)&v14 + 1) = 0i64;
    std::_Tree_temp_node<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>::~_Tree_temp_node<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>(&v14);
    v14 = v7;
    *(_QWORD *)a2 = std::_Tree_val<std::_Tree_simple_types<std::pair<std::wstring const,cxl::PropertyList::Property>>>::_Insert_node(a1, &v14, v11);
    *(_BYTE *)(a2 + 8) = 1;
  }
  return a2;
}


==========

FUNCTION: ??$_Emplace_back_with_unused_capacity@U?$pair@_K_K@std@@@?$vector@U?$pair@_K_K@std@@V?$allocator@U?$pair@_K_K@std@@@2@@std@@AEAAAEAU?$pair@_K_K@1@$$QEAU21@@Z @ 0x1800146B4
----------
__int64 __fastcall std::vector<std::pair<unsigned __int64,unsigned __int64>>::_Emplace_back_with_unused_capacity<std::pair<unsigned __int64,unsigned __int64>>(__int64 a1, _OWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::_Construct_in_place<std::pair<unsigned __int64,unsigned __int64>,std::pair<unsigned __int64,unsigned __int64>>(*(_OWORD **)(a1 + 8), a2);
  result = *(_QWORD *)(v2 + 8);
  *(_QWORD *)(v2 + 8) = result + 16;
  return result;
}


==========

FUNCTION: ??$_Emplace_reallocate@U?$pair@_K_K@std@@@?$vector@U?$pair@_K_K@std@@V?$allocator@U?$pair@_K_K@std@@@2@@std@@AEAAPEAU?$pair@_K_K@1@QEAU21@$$QEAU21@@Z @ 0x1800146DC
----------
char *__fastcall std::vector<std::pair<unsigned __int64,unsigned __int64>>::_Emplace_reallocate<std::pair<unsigned __int64,unsigned __int64>>(__int64 a1, char *a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v6 = (unsigned __int64)&a2[-*(_QWORD *)a1];
  v7 = (__int64)(*(_QWORD *)(a1 + 8) - *(_QWORD *)a1) /*signed*/>> 4;
  v8 = 0xFFFFFFFFFFFFFFFi64;
  if ( v7 == 0xFFFFFFFFFFFFFFFi64 )
    std::_Xlength_error("vector too long");
  v9 = v7 + 1;
  v10 = (__int64)(*(_QWORD *)(a1 + 16) - *(_QWORD *)a1) /*signed*/>> 4;
  v11 = v10 >> 1;
  if ( v10 <= 0xFFFFFFFFFFFFFFFi64 - (v10 >> 1) )
  {
    v8 = v11 + v10;
    if ( v11 + v10 < v9 )
      v8 = v9;
  }
  v21 = (_QWORD *)v8;
  v12 = std::_Allocate_at_least_helper<std::allocator<std::pair<unsigned __int64,unsigned __int64>>>(v10, &v21);
  v21 = v12;
  v13 = (char *)v12 + (v6 & 0xFFFFFFFFFFFFFFF0ui64);
  v15 = (void *)std::_Default_allocator_traits<std::allocator<std::pair<unsigned __int64,unsigned __int64>>>::construct<std::pair<unsigned __int64,unsigned __int64>,std::pair<unsigned __int64,unsigned __int64>>(v14, v13, a3);
  v16 = *(char **)(a1 + 8);
  v17 = *(_BYTE **)a1;
  v18 = v15;
  if ( a2 == v16 )
  {
    v19 = v16 - v17;
  }
  else
  {
    memmove_0(v15, v17, (size_t)&a2[-*(_QWORD *)a1]);
    v18 = v13 + 16;
    v19 = *(_QWORD *)(a1 + 8) - (_QWORD)a2;
    v17 = a2;
  }
  memmove_0(v18, v17, v19);
  if ( *(_QWORD *)a1 )
    std::_Deallocate<16,0>(*(void **)a1, (*(_QWORD *)(a1 + 16) - *(_QWORD *)a1) & 0xFFFFFFFFFFFFFFF0ui64);
  *(_QWORD *)a1 = v12;
  *(_QWORD *)(a1 + 8) = &v12[2 * v9];
  *(_QWORD *)(a1 + 16) = &v12[2 * v8];
  return v13;
}


==========

FUNCTION: ??$_Emplace_reallocate@V?$tuple@_KUProperty@PropertyList@cxl@@_N@std@@@?$vector@V?$tuple@_KUProperty@PropertyList@cxl@@_N@std@@V?$allocator@V?$tuple@_KUProperty@PropertyList@cxl@@_N@std@@@2@@std@@AEAAPEAV?$tuple@_KUProperty@PropertyList@cxl@@_N@1@QEAV21@$$QEAV21@@Z @ 0x180014810
----------
char *__fastcall std::vector<std::tuple<unsigned __int64,cxl::PropertyList::Property,bool>>::_Emplace_reallocate<std::tuple<unsigned __int64,cxl::PropertyList::Property,bool>>(__int64 *a1, void *a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v6 = (__int64)a2 - *a1;
  v7 = (a1[1] - *a1) /*signed*/>> 5;
  v8 = 0x7FFFFFFFFFFFFFFi64;
  if ( v7 == 0x7FFFFFFFFFFFFFFi64 )
    std::_Xlength_error("vector too long");
  v9 = v7 + 1;
  v10 = (a1[2] - *a1) /*signed*/>> 5;
  v11 = v10 >> 1;
  if ( v10 <= 0x7FFFFFFFFFFFFFFi64 - (v10 >> 1) )
  {
    v8 = v11 + v10;
    if ( v11 + v10 < v9 )
      v8 = v9;
  }
  v19 = (_QWORD *)v8;
  v12 = std::_Allocate_at_least_helper<std::allocator<std::wstring>>(v10, &v19);
  v19 = v12;
  v13 = (char *)v12 + (v6 & 0xFFFFFFFFFFFFFFE0ui64);
  std::_Default_allocator_traits<std::allocator<std::tuple<unsigned __int64,cxl::PropertyList::Property,bool>>>::construct<std::tuple<unsigned __int64,cxl::PropertyList::Property,bool>,std::tuple<unsigned __int64,cxl::PropertyList::Property,bool>>(v14, v13, a3);
  v15 = (void *)a1[1];
  v16 = v12;
  v17 = *a1;
  if ( a2 != v15 )
  {
    std::_Uninitialized_move<std::tuple<unsigned __int64,cxl::PropertyList::Property,bool> *,std::allocator<std::tuple<unsigned __int64,cxl::PropertyList::Property,bool>>>(v17, a2, v12);
    v16 = v13 + 32;
    v15 = (void *)a1[1];
    v17 = (__int64)a2;
  }
  std::_Uninitialized_move<std::tuple<unsigned __int64,cxl::PropertyList::Property,bool> *,std::allocator<std::tuple<unsigned __int64,cxl::PropertyList::Property,bool>>>(v17, v15, v16);
  if ( *a1 )
    std::_Deallocate<16,0>((void *)*a1, (a1[2] - *a1) & 0xFFFFFFFFFFFFFFE0ui64);
  *a1 = (__int64)v12;
  a1[1] = (__int64)&v12[4 * v9];
  a1[2] = (__int64)&v12[4 * v8];
  return v13;
}


==========

FUNCTION: ??$_Erase_head@V?$allocator@U?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@std@@@std@@@?$_Tree_val@U?$_Tree_simple_types@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@@std@@@std@@QEAAXAEAV?$allocator@U?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@std@@@1@@Z @ 0x180014938
----------
void __fastcall std::_Tree_val<std::_Tree_simple_types<std::pair<std::wstring const,cxl::PropertyList::Property>>>::_Erase_head<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>(void **a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::_Tree_val<std::_Tree_simple_types<std::pair<std::wstring const,cxl::PropertyList::Property>>>::_Erase_tree_and_orphan<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>((__int64)a1, a2, *((__int64 **)*a1 + 1));
  std::_Deallocate<16,0>(*a1, 0x50ui64);
}


==========

FUNCTION: ??$_Find@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@?$_Tree@V?$_Tmap_traits@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@UCaseInsensitive_LessThan@5@V?$allocator@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@@2@$0A@@std@@@std@@AEBAPEAU?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@1@AEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@1@@Z @ 0x180014968
----------
__int64 __fastcall std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::_Find<std::wstring>(__int64 *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::_Find_lower_bound<std::wstring>(a1, v7);
  v5 = (unsigned __int8)std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::_Lower_bound_duplicate<std::wstring>(v4, v8, a2) == 0;
  result = v8;
  if ( v5 )
    result = *a1;
  return result;
}


==========

FUNCTION: ??$_Find_lower_bound@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@?$_Tree@V?$_Tmap_traits@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@UCaseInsensitive_LessThan@5@V?$allocator@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@@2@$0A@@std@@@std@@IEBA?AU?$_Tree_find_result@PEAU?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@std@@@1@AEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@1@@Z @ 0x1800149B0
----------
__int64 **__fastcall std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::_Find_lower_bound<std::wstring>(__int64 a1, __int64 **a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v5 = *(__int64 **)(*(_QWORD *)a1 + 8i64);
  *((_DWORD *)a2 + 2) = 0;
  *a2 = v5;
  *((_DWORD *)a2 + 3) = 0;
  v6 = *a2;
  a2[2] = *(__int64 **)a1;
  while ( !*((_BYTE *)v6 + 25) )
  {
    *a2 = v6;
    if ( (unsigned __int8)cxl::CaseInsensitive_LessThan::operator()(a1, v6 + 4, a3) )
    {
      *((_DWORD *)a2 + 2) = 0;
      v6 = (__int64 *)v6[2];
    }
    else
    {
      *((_DWORD *)a2 + 2) = 1;
      a2[2] = v6;
      v6 = (__int64 *)*v6;
    }
  }
  return a2;
}


==========

FUNCTION: ??$_Lower_bound_duplicate@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@?$_Tree@V?$_Tmap_traits@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@UCaseInsensitive_LessThan@5@V?$allocator@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@@2@$0A@@std@@@std@@IEBA_NQEAU?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@1@AEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@1@@Z @ 0x180014A30
----------
char __fastcall std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::_Lower_bound_duplicate<std::wstring>(__int64 a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = 0;
  if ( !*(_BYTE *)(a2 + 25) && !cxl::CaseInsensitive_LessThan::operator()((struct cxl::CaseInsensitive_LessThan *)a1, a3) )
    v3 = 1;
  return v3;
}


==========

FUNCTION: ??$_Resize_reallocate@U_Value_init_tag@std@@@?$vector@EV?$allocator@E@std@@@std@@AEAAX_KAEBU_Value_init_tag@1@@Z @ 0x180014A64
----------
__int64 __fastcall std::vector<unsigned char>::_Resize_reallocate<std::_Value_init_tag>(const void **a1, unsigned __int64 a2, size_t a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v10 = a3;
  if ( a2 > 0x7FFFFFFFFFFFFFFFi64 )
    std::vector<wchar_t>::_Xlength();
  v5 = (_BYTE *)a1[1] - (_BYTE *)*a1;
  v6 = std::vector<unsigned char>::_Calculate_growth(a1, a2);
  v10 = v6;
  v8 = std::_Allocate_at_least_helper<std::allocator<unsigned char>>(v7, &v10);
  memset_0((char *)v8 + v5, 0, a2 - v5);
  memmove_0(v8, *a1, (_BYTE *)a1[1] - (_BYTE *)*a1);
  return std::vector<unsigned char>::_Change_array(a1, v8, a2, v6);
}


==========

FUNCTION: ??$_Resize_reallocate@U_Value_init_tag@std@@@?$vector@_WV?$allocator@_W@std@@@std@@AEAAX_KAEBU_Value_init_tag@1@@Z @ 0x180014B0C
----------
__int64 __fastcall std::vector<wchar_t>::_Resize_reallocate<std::_Value_init_tag>(const void **a1, unsigned __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v10 = a3;
  if ( a2 > 0x7FFFFFFFFFFFFFFFi64 )
    std::vector<wchar_t>::_Xlength();
  v5 = ((_BYTE *)a1[1] - (_BYTE *)*a1) /*signed*/>> 1;
  v6 = std::vector<wchar_t>::_Calculate_growth(a1, a2);
  v10 = v6;
  v8 = std::_Allocate_at_least_helper<std::allocator<wchar_t>>(v7, &v10);
  memset_0((char *)v8 + 2 * v5, 0, 2 * (a2 - v5));
  memmove_0(v8, *a1, (_BYTE *)a1[1] - (_BYTE *)*a1);
  return std::vector<wchar_t>::_Change_array((__int64)a1, (__int64)v8, a2, v6);
}


==========

FUNCTION: ??$_Uninitialized_move@PEAV?$tuple@_KUProperty@PropertyList@cxl@@_N@std@@V?$allocator@V?$tuple@_KUProperty@PropertyList@cxl@@_N@std@@@2@@std@@YAPEAV?$tuple@_KUProperty@PropertyList@cxl@@_N@0@QEAV10@0PEAV10@AEAV?$allocator@V?$tuple@_KUProperty@PropertyList@cxl@@_N@std@@@0@@Z @ 0x180014BBC
----------
__int64 __fastcall std::_Uninitialized_move<std::tuple<unsigned __int64,cxl::PropertyList::Property,bool> *,std::allocator<std::tuple<unsigned __int64,cxl::PropertyList::Property,bool>>>(__int64 a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = a1;
  if ( a1 != a2 )
  {
    do
    {
      std::_Default_allocator_traits<std::allocator<std::tuple<unsigned __int64,cxl::PropertyList::Property,bool>>>::construct<std::tuple<unsigned __int64,cxl::PropertyList::Property,bool>,std::tuple<unsigned __int64,cxl::PropertyList::Property,bool>>(a1, a3, v4);
      a3 = v5 + 32;
      v4 = v6 + 32;
    }
    while ( v4 != v7 );
  }
  return a3;
}


==========

FUNCTION: ??$construct@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@U?$pair@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@2@@?$_Default_allocator_traits@V?$allocator@U?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@std@@@std@@@std@@SAXAEAV?$allocator@U?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@std@@@1@QEAU?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@1@$$QEAU?$pair@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@1@@Z @ 0x180014BF4
----------
__int64 __fastcall std::_Default_allocator_traits<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>::construct<std::pair<std::wstring const,cxl::PropertyList::Property>,std::pair<std::wstring,cxl::PropertyList::Property>>(__int64 a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  result = 0i64;
  *(_OWORD *)a2 = 0i64;
  *(_QWORD *)(a2 + 16) = 0i64;
  *(_QWORD *)(a2 + 24) = 0i64;
  *(_OWORD *)a2 = *(_OWORD *)a3;
  *(_OWORD *)(a2 + 16) = *(_OWORD *)(a3 + 16);
  *(_QWORD *)(a3 + 16) = 0i64;
  *(_QWORD *)(a3 + 24) = 7i64;
  *(_WORD *)a3 = 0;
  *(_OWORD *)(a2 + 32) = *(_OWORD *)(a3 + 32);
  return result;
}


==========

FUNCTION: ??$construct@U?$pair@_K_K@std@@U12@@?$_Default_allocator_traits@V?$allocator@U?$pair@_K_K@std@@@std@@@std@@SAXAEAV?$allocator@U?$pair@_K_K@std@@@1@QEAU?$pair@_K_K@1@$$QEAU31@@Z @ 0x180014C38
----------
void __fastcall std::_Default_allocator_traits<std::allocator<std::pair<unsigned __int64,unsigned __int64>>>::construct<std::pair<unsigned __int64,unsigned __int64>,std::pair<unsigned __int64,unsigned __int64>>(__int64 a1, _OWORD *a2, _OWORD *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a2 = *a3;
}


==========

FUNCTION: ??$construct@V?$tuple@_KUProperty@PropertyList@cxl@@_N@std@@V12@@?$_Default_allocator_traits@V?$allocator@V?$tuple@_KUProperty@PropertyList@cxl@@_N@std@@@std@@@std@@SAXAEAV?$allocator@V?$tuple@_KUProperty@PropertyList@cxl@@_N@std@@@1@QEAV?$tuple@_KUProperty@PropertyList@cxl@@_N@1@$$QEAV31@@Z @ 0x180014C48
----------
__int64 __fastcall std::_Default_allocator_traits<std::allocator<std::tuple<unsigned __int64,cxl::PropertyList::Property,bool>>>::construct<std::tuple<unsigned __int64,cxl::PropertyList::Property,bool>,std::tuple<unsigned __int64,cxl::PropertyList::Property,bool>>(__int64 a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_BYTE *)a2 = *(_BYTE *)a3;
  *(_OWORD *)(a2 + 8) = *(_OWORD *)(a3 + 8);
  result = *(_QWORD *)(a3 + 24);
  *(_QWORD *)(a2 + 24) = result;
  return result;
}


==========

FUNCTION: ??$fill_n@PEAE_KE@std@@YAPEAEPEAE_KAEBE@Z @ 0x180014C68
----------
char *__fastcall std::fill_n<unsigned char *,unsigned __int64,unsigned char>(char *a1, size_t a2, unsigned __int8 *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !a2 )
    return a1;
  memset_0(a1, *a3, a2);
  return &a1[a2];
}


==========

FUNCTION: std::for_each_std::_Vector_iterator_std::_Vector_val_std::_Simple_types_std::pair_unsigned___int64_unsigned___int64_________lambda_d5fa9e7f269ff9f849813ad7aa6b7140___ @ 0x180014CA4
----------
__int64 __fastcall std::for_each_std::_Vector_iterator_std::_Vector_val_std::_Simple_types_std::pair_unsigned___int64_unsigned___int64_________lambda_d5fa9e7f269ff9f849813ad7aa6b7140___(__int64 a1, _QWORD *a2, _QWORD *a3, __int64 *a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v6 = a2;
  if ( a2 != a3 )
  {
    v8 = *a4;
    v9 = (_QWORD *)a4[2];
    v10 = a4[1];
    do
    {
      v11 = v6[1] - *v6;
      v12 = (_BYTE *)(*v9 + *(_QWORD *)(v8 + 16));
      v16 = 0;
      std::vector<unsigned char>::insert((void **)(v8 + 16), &v17, v12, v11, &v16);
      memcpy_s((void *const)(*v9 + *(_QWORD *)(v8 + 16)), *(_QWORD *)(v8 + 24) - *(_QWORD *)(v8 + 16) - *v9 - 4i64, (const void *const)(*v6 + v10), v6[1] - *v6);
      v13 = v6[1] - *v6;
      v6 += 2;
      *v9 += v13;
    }
    while ( v6 != a3 );
  }
  result = a1;
  v15 = a4[2];
  *(_OWORD *)a1 = *(_OWORD *)a4;
  *(_QWORD *)(a1 + 16) = v15;
  return result;
}


==========

FUNCTION: ??$insert@U?$pair@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@$0A@@?$map@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@UCaseInsensitive_LessThan@5@V?$allocator@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@@2@@std@@QEAA?AU?$pair@V?$_Tree_iterator@V?$_Tree_val@U?$_Tree_simple_types@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@@std@@@std@@@std@@_N@1@$$QEAU?$pair@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@1@@Z @ 0x180014D70
----------
__int64 __fastcall std::map<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>>::insert<std::pair<std::wstring,cxl::PropertyList::Property>,0>(__int64 *a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::_Emplace<std::pair<std::wstring,cxl::PropertyList::Property>>(a1, (__int64)&v5, a3);
  *(_QWORD *)a2 = v5;
  *(_BYTE *)(a2 + 8) = v6;
  return a2;
}


==========

FUNCTION: ??$make_pair@AEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEAUProperty@PropertyList@cxl@@@std@@YA?AU?$pair@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@0@AEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@0@AEAUProperty@PropertyList@cxl@@@Z @ 0x180014DA4
----------
__int64 __fastcall std::make_pair<std::wstring const &,cxl::PropertyList::Property &>(__int64 a1, __int64 a2, _OWORD *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::wstring::wstring((_QWORD *)a1, a2);
  result = a1;
  *(_OWORD *)(a1 + 32) = *a3;
  return result;
}


==========

FUNCTION: ??0?$function@$$A6APEAUCLUSPROP_VALUE@@_K0@Z@std@@QEAA@AEBV01@@Z @ 0x180014DD8
----------
__int64 __fastcall std::function<CLUSPROP_VALUE * (unsigned __int64,unsigned __int64)>::function<CLUSPROP_VALUE * (unsigned __int64,unsigned __int64)>(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)(a1 + 56) = 0i64;
  v3 = *(__int64 (__fastcall ****)(_QWORD, __int64))(a2 + 56);
  if ( v3 )
    *(_QWORD *)(a1 + 56) = (**v3)(v3, a1);
  return a1;
}


==========

FUNCTION: ??0?$map@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@UCaseInsensitive_LessThan@5@V?$allocator@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@@2@@std@@QEAA@XZ @ 0x180014E18
----------
_QWORD *__fastcall std::map<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>>::map<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>>(_QWORD *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = 0i64;
  a1[1] = 0i64;
  *a1 = std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>::_Buyheadnode<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>();
  return a1;
}


==========

FUNCTION: ??0?$shared_ptr@VMiApplication@mi@@@std@@QEAA@AEBV01@@Z @ 0x180014E44
----------
_QWORD *__fastcall std::shared_ptr<mi::MiApplication>::shared_ptr<mi::MiApplication>(_QWORD *a1, _QWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = 0i64;
  a1[1] = 0i64;
  v2 = a2[1];
  if ( v2 )
    _InterlockedIncrement((volatile signed __int32 *)(v2 + 8));
  *a1 = *a2;
  a1[1] = a2[1];
  return a1;
}


==========

FUNCTION: ??1?$_Tidy_guard@V?$vector@EV?$allocator@E@std@@@std@@@std@@QEAA@XZ @ 0x18001544C
----------
void __fastcall std::_Tidy_guard<std::vector<unsigned char>>::~_Tidy_guard<std::vector<unsigned char>>(__int64 *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v1 = *a1;
  if ( v1 )
    std::vector<unsigned char>::_Tidy(v1);
}


==========

FUNCTION: ??1?$_Tree_temp_node@V?$allocator@U?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@std@@@std@@@std@@QEAA@XZ @ 0x18001546C
----------
void __fastcall std::_Tree_temp_node<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>::~_Tree_temp_node<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(_QWORD *)(a1 + 8);
  if ( v2 )
    std::wstring::_Tidy_deallocate(v2 + 32);
  std::_Alloc_construct_ptr<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>::~_Alloc_construct_ptr<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>(a1);
}


==========

FUNCTION: ??1?$map@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@UCaseInsensitive_LessThan@5@V?$allocator@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@@2@@std@@QEAA@XZ @ 0x18001549C
----------
void __fastcall std::map<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>>::~map<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>>(void **a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::_Tree_val<std::_Tree_simple_types<std::pair<std::wstring const,cxl::PropertyList::Property>>>::_Erase_head<std::allocator<std::_Tree_node<std::pair<std::wstring const,cxl::PropertyList::Property>,void *>>>(a1, (__int64)a1);
}


==========

FUNCTION: ??1?$vector@U?$pair@_K_K@std@@V?$allocator@U?$pair@_K_K@std@@@2@@std@@QEAA@XZ @ 0x1800154AC
----------
void __fastcall std::vector<std::pair<unsigned __int64,unsigned __int64>>::~vector<std::pair<unsigned __int64,unsigned __int64>>(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(void **)a1;
  if ( v2 )
  {
    std::_Deallocate<16,0>(v2, (*(_QWORD *)(a1 + 16) - (_QWORD)v2) & 0xFFFFFFFFFFFFFFF0ui64);
    *(_QWORD *)a1 = 0i64;
    *(_QWORD *)(a1 + 8) = 0i64;
    *(_QWORD *)(a1 + 16) = 0i64;
  }
}


==========

FUNCTION: ??1?$vector@V?$tuple@_KUProperty@PropertyList@cxl@@_N@std@@V?$allocator@V?$tuple@_KUProperty@PropertyList@cxl@@_N@std@@@2@@std@@QEAA@XZ @ 0x1800154E8
----------
void __fastcall std::vector<std::tuple<unsigned __int64,cxl::PropertyList::Property,bool>>::~vector<std::tuple<unsigned __int64,cxl::PropertyList::Property,bool>>(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(void **)a1;
  if ( v2 )
  {
    std::_Deallocate<16,0>(v2, (*(_QWORD *)(a1 + 16) - (_QWORD)v2) & 0xFFFFFFFFFFFFFFE0ui64);
    *(_QWORD *)a1 = 0i64;
    *(_QWORD *)(a1 + 8) = 0i64;
    *(_QWORD *)(a1 + 16) = 0i64;
  }
}


==========

FUNCTION: ??4?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@QEAAAEAV01@$$QEAV01@@Z @ 0x180015574
----------
_OWORD *__fastcall std::wstring::operator=(_OWORD *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a1 == (_OWORD *)a2 )
    return a1;
  std::wstring::_Tidy_deallocate((__int64)a1);
  *a1 = *(_OWORD *)a2;
  a1[1] = *(_OWORD *)(a2 + 16);
  *(_QWORD *)(a2 + 16) = 0i64;
  *(_QWORD *)(a2 + 24) = 7i64;
  *(_WORD *)a2 = 0;
  return a1;
}


==========

FUNCTION: ??4?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@QEAAAEAV01@AEBV01@@Z @ 0x1800155C4
----------
__int64 __fastcall std::wstring::operator=(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a1 == a2 )
    return a1;
  v3 = std::_String_val<std::_Simple_types<wchar_t>>::_Myptr(a2);
  std::wstring::assign(a1, v3, *(_QWORD *)(v4 + 16));
  return a1;
}


==========

FUNCTION: ??4?$function@$$A6APEAUCLUSPROP_VALUE@@_K0@Z@std@@QEAAAEAV01@AEBV01@@Z @ 0x1800155FC
----------
__int64 __fastcall std::function<CLUSPROP_VALUE * (unsigned __int64,unsigned __int64)>::operator=(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = std::function<CLUSPROP_VALUE * (unsigned __int64,unsigned __int64)>::function<CLUSPROP_VALUE * (unsigned __int64,unsigned __int64)>((__int64)v11, a2);
  v5 = *(_QWORD *)(v4 + 56);
  if ( v5 == v4 || (v6 = *(_QWORD *)(a1 + 56), v6 == a1) )
  {
    v10 = 0i64;
    std::_Func_class<CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::_Reset_move(v9, v4);
    std::_Func_class<CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::_Reset_move(v4, a1);
    std::_Func_class<CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::_Reset_move(a1, v9);
    std::function<void (CLUSPROP_ULARGE_INTEGER *)>::~function<void (CLUSPROP_ULARGE_INTEGER *)>((__int64)v9, v7);
  }
  else
  {
    *(_QWORD *)(v4 + 56) = v6;
    *(_QWORD *)(a1 + 56) = v5;
  }
  std::function<void (CLUSPROP_ULARGE_INTEGER *)>::~function<void (CLUSPROP_ULARGE_INTEGER *)>((__int64)v11, v3);
  return a1;
}


==========

FUNCTION: ??4?$shared_ptr@VMiApplication@mi@@@std@@QEAAAEAV01@AEBV01@@Z @ 0x1800156B4
----------
__int64 *__fastcall std::shared_ptr<mi::MiApplication>::operator=(__int64 *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = (__int64 *)std::shared_ptr<mi::MiApplication>::shared_ptr<mi::MiApplication>(v7);
  v3 = *v2;
  *v2 = *a1;
  *a1 = v3;
  v4 = v2[1];
  v2[1] = a1[1];
  v5 = v8;
  a1[1] = v4;
  if ( v5 )
    std::_Ref_count_base::_Decref(v5);
  return a1;
}


==========

FUNCTION: ??4?$vector@EV?$allocator@E@std@@@std@@QEAAAEAV01@$$QEAV01@@Z @ 0x180015704
----------
_QWORD *__fastcall std::vector<unsigned char>::operator=(_QWORD *a1, _QWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a1 == a2 )
    return a1;
  std::vector<unsigned char>::_Tidy((__int64)a1);
  *a1 = *a2;
  a1[1] = a2[1];
  a1[2] = a2[2];
  *a2 = 0i64;
  a2[1] = 0i64;
  a2[2] = 0i64;
  return a1;
}


==========

FUNCTION: ??E?$_Tree_unchecked_const_iterator@V?$_Tree_val@U?$_Tree_simple_types@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@@std@@@std@@U_Iterator_base0@2@@std@@QEAAAEAV01@XZ @ 0x1800159CC
----------
_QWORD *__fastcall std::_Tree_unchecked_const_iterator<std::_Tree_val<std::_Tree_simple_types<std::pair<std::wstring const,cxl::PropertyList::Property>>>,std::_Iterator_base0>::operator++(_QWORD *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = (_QWORD *)*a1;
  if ( *(_BYTE *)(v2[2] + 25i64) )
  {
    for ( i = (_QWORD *)v2[1]; !*((_BYTE *)i + 25) && v2 == (_QWORD *)i[2]; i = (_QWORD *)i[1] )
    {
      *a1 = i;
      v2 = i;
    }
  }
  else
  {
    i = std::_Tree_val<std::_Tree_simple_types<std::pair<std::wstring const,cxl::PropertyList::Property>>>::_Min((_QWORD *)v2[2]);
  }
  *a1 = i;
  return a1;
}


==========

FUNCTION: ??R?$_Func_class@PEAUCLUSPROP_VALUE@@_K_K@std@@QEBAPEAUCLUSPROP_VALUE@@_K0@Z @ 0x180015E1C
----------
__int64 __fastcall std::_Func_class<CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::operator()(__int64 a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v6 = a3;
  v5 = a2;
  v3 = *(_QWORD *)(a1 + 56);
  if ( v3 )
    return (*(__int64 (__fastcall **)(__int64, __int64 *, __int64 *))(*(_QWORD *)v3 + 16i64))(v3, &v5, &v6);
  std::_Xbad_function_call();
  __debugbreak();
  return (*(__int64 (__fastcall **)(__int64, __int64 *, __int64 *))(*(_QWORD *)v3 + 16i64))(v3, &v5, &v6);
}


==========

FUNCTION: ??R?$_Func_class@XPEAUCLUSPROP_SZ@@@std@@QEBAXPEAUCLUSPROP_SZ@@@Z @ 0x180015E64
----------
__int64 __fastcall std::_Func_class<void,CLUSPROP_SZ *>::operator()(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = a2;
  v2 = *(_QWORD *)(a1 + 56);
  if ( v2 )
    return (*(__int64 (__fastcall **)(__int64, __int64 *))(*(_QWORD *)v2 + 16i64))(v2, &v4);
  std::_Xbad_function_call();
  __debugbreak();
  return (*(__int64 (__fastcall **)(__int64, __int64 *))(*(_QWORD *)v2 + 16i64))(v2, &v4);
}


==========

FUNCTION: ??_G?$_Ref_count_obj2@V?$ValueElement@GUCLUSPROP_WORD@@@ValueList@cxl@@@std@@UEAAPEAXI@Z @ 0x180016320
----------
_QWORD *__fastcall std::_Ref_count_obj2<cxl::ValueList::ValueElement<unsigned short,CLUSPROP_WORD>>::`scalar deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &std::_Ref_count_obj2<cxl::ValueList::ValueElement<unsigned short,CLUSPROP_WORD>>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_E?$_Ref_count_obj2@V?$ValueElement@JUCLUSPROP_LONG@@@ValueList@cxl@@@std@@UEAAPEAXI@Z @ 0x180016360
----------
_QWORD *__fastcall std::_Ref_count_obj2<cxl::ValueList::ValueElement<long,CLUSPROP_LONG>>::`vector deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &std::_Ref_count_obj2<cxl::ValueList::ValueElement<long,CLUSPROP_LONG>>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_G?$_Ref_count_obj2@V?$ValueElement@KUCLUSPROP_DWORD@@@ValueList@cxl@@@std@@UEAAPEAXI@Z @ 0x1800163A0
----------
_QWORD *__fastcall std::_Ref_count_obj2<cxl::ValueList::ValueElement<unsigned long,CLUSPROP_DWORD>>::`scalar deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &std::_Ref_count_obj2<cxl::ValueList::ValueElement<unsigned long,CLUSPROP_DWORD>>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_G?$_Ref_count_obj2@V?$ValueElement@T_LARGE_INTEGER@@UCLUSPROP_LARGE_INTEGER@@@ValueList@cxl@@@std@@UEAAPEAXI@Z @ 0x1800163E0
----------
_QWORD *__fastcall std::_Ref_count_obj2<cxl::ValueList::ValueElement<_LARGE_INTEGER,CLUSPROP_LARGE_INTEGER>>::`scalar deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &std::_Ref_count_obj2<cxl::ValueList::ValueElement<_LARGE_INTEGER,CLUSPROP_LARGE_INTEGER>>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_E?$_Ref_count_obj2@V?$ValueElement@T_ULARGE_INTEGER@@UCLUSPROP_ULARGE_INTEGER@@@ValueList@cxl@@@std@@UEAAPEAXI@Z @ 0x180016420
----------
_QWORD *__fastcall std::_Ref_count_obj2<cxl::ValueList::ValueElement<_ULARGE_INTEGER,CLUSPROP_ULARGE_INTEGER>>::`vector deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &std::_Ref_count_obj2<cxl::ValueList::ValueElement<_ULARGE_INTEGER,CLUSPROP_ULARGE_INTEGER>>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_E?$_Ref_count_obj2@V?$ValueElement@U_FILETIME@@UCLUSPROP_FILETIME@@@ValueList@cxl@@@std@@UEAAPEAXI@Z @ 0x180016460
----------
_QWORD *__fastcall std::_Ref_count_obj2<cxl::ValueList::ValueElement<_FILETIME,CLUSPROP_FILETIME>>::`vector deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &std::_Ref_count_obj2<cxl::ValueList::ValueElement<_FILETIME,CLUSPROP_FILETIME>>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_G?$_Ref_count_obj2@V?$ValueElement@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UCLUSPROP_SZ@@@ValueList@cxl@@@std@@UEAAPEAXI@Z @ 0x1800164A0
----------
_QWORD *__fastcall std::_Ref_count_obj2<cxl::ValueList::ValueElement<std::wstring,CLUSPROP_SZ>>::`scalar deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &std::_Ref_count_obj2<cxl::ValueList::ValueElement<std::wstring,CLUSPROP_SZ>>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_G?$_Ref_count_obj2@V?$ValueElement@V?$vector@EV?$allocator@E@std@@@std@@UCLUSPROP_BINARY@@@ValueList@cxl@@@std@@UEAAPEAXI@Z @ 0x1800164E0
----------
_QWORD *__fastcall std::_Ref_count_obj2<cxl::ValueList::ValueElement<std::vector<unsigned char>,CLUSPROP_BINARY>>::`scalar deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &std::_Ref_count_obj2<cxl::ValueList::ValueElement<std::vector<unsigned char>,CLUSPROP_BINARY>>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_G?$_Ref_count_obj2@V?$ValueElement@V?$vector@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@V?$allocator@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@@2@@std@@UCLUSPROP_SZ@@@ValueList@cxl@@@std@@UEAAPEAXI@Z @ 0x180016520
----------
_QWORD *__fastcall std::_Ref_count_obj2<cxl::ValueList::ValueElement<std::vector<std::wstring>,CLUSPROP_SZ>>::`scalar deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &std::_Ref_count_obj2<cxl::ValueList::ValueElement<std::vector<std::wstring>,CLUSPROP_SZ>>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ?_Buy_nonzero@?$vector@EV?$allocator@E@std@@@std@@AEAAX_K@Z @ 0x180017434
----------
char *__fastcall std::vector<unsigned char>::_Buy_nonzero(_QWORD *a1, size_t a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a2 > 0x7FFFFFFFFFFFFFFFi64 )
    std::vector<wchar_t>::_Xlength();
  return std::vector<unsigned char>::_Buy_raw(a1, a2);
}


==========

FUNCTION: ?_Change_array@?$vector@EV?$allocator@E@std@@@std@@AEAAXQEAE_K1@Z @ 0x18001745C
----------
__int64 __fastcall std::vector<unsigned char>::_Change_array(__int64 a1, __int64 a2, __int64 a3, __int64 a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v6 = *(void **)a1;
  if ( v6 )
    std::_Deallocate<16,0>(v6, *(_QWORD *)(a1 + 16) - (_QWORD)v6);
  *(_QWORD *)a1 = a2;
  *(_QWORD *)(a1 + 8) = a2 + a3;
  result = a2 + a4;
  *(_QWORD *)(a1 + 16) = a2 + a4;
  return result;
}


==========

FUNCTION: ?_Move@?$_Func_impl_no_alloc@V_lambda_061c9011cb9bb82bcf658bf6b36e3ac2_@@PEAUCLUSPROP_VALUE@@_K_K@std@@EEAAPEAV?$_Func_base@PEAUCLUSPROP_VALUE@@_K_K@2@PEAX@Z @ 0x1800174C0
----------
__int64 __fastcall std::_Func_impl_no_alloc<_lambda_061c9011cb9bb82bcf658bf6b36e3ac2_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::_Move(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)a2 = &std::_Func_impl_no_alloc<_lambda_061c9011cb9bb82bcf658bf6b36e3ac2_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::`vftable';
  result = a2;
  *(_OWORD *)(a2 + 8) = *(_OWORD *)(a1 + 8);
  *(_QWORD *)(a2 + 24) = *(_QWORD *)(a1 + 24);
  return result;
}


==========

FUNCTION: ?_Copy@?$_Func_impl_no_alloc@V_lambda_13bf9359f506f7ffd00982af1d5f3a49_@@PEAUCLUSPROP_VALUE@@_K_K@std@@EEBAPEAV?$_Func_base@PEAUCLUSPROP_VALUE@@_K_K@2@PEAX@Z @ 0x180017510
----------
__int64 __fastcall std::_Func_impl_no_alloc<_lambda_13bf9359f506f7ffd00982af1d5f3a49_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::_Copy(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)a2 = &std::_Func_impl_no_alloc<_lambda_13bf9359f506f7ffd00982af1d5f3a49_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::`vftable';
  result = a2;
  *(_OWORD *)(a2 + 8) = *(_OWORD *)(a1 + 8);
  *(_QWORD *)(a2 + 24) = *(_QWORD *)(a1 + 24);
  return result;
}


==========

FUNCTION: ?_Move@?$_Func_impl_no_alloc@V_lambda_37f697dbc2d2dfb7d8e4239a9e92d6a3_@@PEAUCLUSPROP_VALUE@@_K_K@std@@EEAAPEAV?$_Func_base@PEAUCLUSPROP_VALUE@@_K_K@2@PEAX@Z @ 0x180017570
----------
__int64 __fastcall std::_Func_impl_no_alloc<_lambda_37f697dbc2d2dfb7d8e4239a9e92d6a3_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::_Move(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)a2 = &std::_Func_impl_no_alloc<_lambda_37f697dbc2d2dfb7d8e4239a9e92d6a3_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::`vftable';
  result = a2;
  *(_OWORD *)(a2 + 8) = *(_OWORD *)(a1 + 8);
  *(_QWORD *)(a2 + 24) = *(_QWORD *)(a1 + 24);
  return result;
}


==========

FUNCTION: ?_Copy@?$_Func_impl_no_alloc@V_lambda_3b4ff329403cc3f130f9ace5ccfa8dfa_@@PEAUCLUSPROP_VALUE@@_K_K@std@@EEBAPEAV?$_Func_base@PEAUCLUSPROP_VALUE@@_K_K@2@PEAX@Z @ 0x1800175A0
----------
__int64 __fastcall std::_Func_impl_no_alloc<_lambda_3b4ff329403cc3f130f9ace5ccfa8dfa_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::_Copy(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)a2 = &std::_Func_impl_no_alloc<_lambda_3b4ff329403cc3f130f9ace5ccfa8dfa_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::`vftable';
  result = a2;
  *(_OWORD *)(a2 + 8) = *(_OWORD *)(a1 + 8);
  *(_QWORD *)(a2 + 24) = *(_QWORD *)(a1 + 24);
  return result;
}


==========

FUNCTION: ?_Move@?$_Func_impl_no_alloc@V_lambda_4ef0909d8e90c100aba481fabd6c4ca0_@@PEAUCLUSPROP_VALUE@@_K_K@std@@EEAAPEAV?$_Func_base@PEAUCLUSPROP_VALUE@@_K_K@2@PEAX@Z @ 0x1800175D0
----------
__int64 __fastcall std::_Func_impl_no_alloc<_lambda_4ef0909d8e90c100aba481fabd6c4ca0_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::_Move(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)a2 = &std::_Func_impl_no_alloc<_lambda_4ef0909d8e90c100aba481fabd6c4ca0_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::`vftable';
  result = a2;
  *(_OWORD *)(a2 + 8) = *(_OWORD *)(a1 + 8);
  *(_QWORD *)(a2 + 24) = *(_QWORD *)(a1 + 24);
  return result;
}


==========

FUNCTION: ?_Copy@?$_Func_impl_no_alloc@V_lambda_b2090a35b16344f94b4d1150e607dfd3_@@PEAUCLUSPROP_VALUE@@_K_K@std@@EEBAPEAV?$_Func_base@PEAUCLUSPROP_VALUE@@_K_K@2@PEAX@Z @ 0x180017640
----------
__int64 __fastcall std::_Func_impl_no_alloc<_lambda_b2090a35b16344f94b4d1150e607dfd3_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::_Copy(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)a2 = &std::_Func_impl_no_alloc<_lambda_b2090a35b16344f94b4d1150e607dfd3_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::`vftable';
  result = a2;
  *(_OWORD *)(a2 + 8) = *(_OWORD *)(a1 + 8);
  *(_QWORD *)(a2 + 24) = *(_QWORD *)(a1 + 24);
  return result;
}


==========

FUNCTION: ?_Copy@?$_Func_impl_no_alloc@V_lambda_d97f77950fffadeda0a44d074e6b4f77_@@PEAUCLUSPROP_VALUE@@_K_K@std@@EEBAPEAV?$_Func_base@PEAUCLUSPROP_VALUE@@_K_K@2@PEAX@Z @ 0x1800176B0
----------
__int64 __fastcall std::_Func_impl_no_alloc<_lambda_d97f77950fffadeda0a44d074e6b4f77_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::_Copy(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)a2 = &std::_Func_impl_no_alloc<_lambda_d97f77950fffadeda0a44d074e6b4f77_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::`vftable';
  result = a2;
  *(_OWORD *)(a2 + 8) = *(_OWORD *)(a1 + 8);
  *(_QWORD *)(a2 + 24) = *(_QWORD *)(a1 + 24);
  return result;
}


==========

FUNCTION: ?_Copy@?$_Func_impl_no_alloc@V_lambda_e03fcb33fc3f30c21ab44e5b90795fb4_@@PEAUCLUSPROP_VALUE@@_K_K@std@@EEBAPEAV?$_Func_base@PEAUCLUSPROP_VALUE@@_K_K@2@PEAX@Z @ 0x180017710
----------
__int64 __fastcall std::_Func_impl_no_alloc<_lambda_e03fcb33fc3f30c21ab44e5b90795fb4_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::_Copy(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)a2 = &std::_Func_impl_no_alloc<_lambda_e03fcb33fc3f30c21ab44e5b90795fb4_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::`vftable';
  result = a2;
  *(_OWORD *)(a2 + 8) = *(_OWORD *)(a1 + 8);
  *(_QWORD *)(a2 + 24) = *(_QWORD *)(a1 + 24);
  return result;
}


==========

FUNCTION: ?_Delete_this@?$_Func_impl_no_alloc@V_lambda_e03fcb33fc3f30c21ab44e5b90795fb4_@@PEAUCLUSPROP_VALUE@@_K_K@std@@EEAAX_N@Z @ 0x1800177B0
----------
void __fastcall std::_Func_impl_no_alloc<_lambda_e03fcb33fc3f30c21ab44e5b90795fb4_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::_Delete_this(void *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a2 )
    operator delete(a1);
}


==========

FUNCTION: ?_Delete_this@?$_Ref_count_obj2@V?$ValueElement@T_ULARGE_INTEGER@@UCLUSPROP_ULARGE_INTEGER@@@ValueList@cxl@@@std@@EEAAXXZ @ 0x1800177D0
----------
__int64 __fastcall std::_Ref_count_obj2<cxl::ValueList::ValueElement<_ULARGE_INTEGER,CLUSPROP_ULARGE_INTEGER>>::_Delete_this(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a1 )
    result = (*(__int64 (__fastcall **)(__int64, __int64))(*(_QWORD *)a1 + 16i64))(a1, 1i64);
  return result;
}


==========

FUNCTION: ?_Destroy@?$_Ref_count_obj2@V?$ValueElement@JUCLUSPROP_LONG@@@ValueList@cxl@@@std@@EEAAXXZ @ 0x180017800
----------
__int64 __fastcall std::_Ref_count_obj2<cxl::ValueList::ValueElement<long,CLUSPROP_LONG>>::_Destroy(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return (*(__int64 (__fastcall **)(__int64, _QWORD))(*(_QWORD *)(a1 + 16) + 40i64))(a1 + 16, 0i64);
}


==========

FUNCTION: ?_Do_call@?$_Func_impl_no_alloc@V_lambda_3b4ff329403cc3f130f9ace5ccfa8dfa_@@PEAUCLUSPROP_VALUE@@_K_K@std@@EEAAPEAUCLUSPROP_VALUE@@$$QEA_K0@Z @ 0x180017820
----------
__int64 __fastcall std::_Func_impl_no_alloc<_lambda_3b4ff329403cc3f130f9ace5ccfa8dfa_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::_Do_call(__int64 a1, __int64 *a2, __int64 *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return _lambda_3b4ff329403cc3f130f9ace5ccfa8dfa_::operator()((cxl::PropertyList **)(a1 + 8), *a2, *a3);
}


==========

FUNCTION: ?_Get_deleter@_Ref_count_base@std@@UEBAPEAXAEBVtype_info@@@Z @ 0x180017A20
----------
void *__fastcall std::_Ref_count_base::_Get_deleter(std::_Ref_count_base *this, const struct type_info *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return 0i64;
}


==========

FUNCTION: ?_Insert_node@?$_Tree_val@U?$_Tree_simple_types@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@@std@@@std@@QEAAPEAU?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@2@U?$_Tree_id@PEAU?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@std@@@2@QEAU32@@Z @ 0x180017A2C
----------
__int64 __fastcall std::_Tree_val<std::_Tree_simple_types<std::pair<std::wstring const,cxl::PropertyList::Property>>>::_Insert_node(__int64 a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  ++*(_QWORD *)(a1 + 8);
  v3 = a3;
  v4 = *(_QWORD **)a1;
  v6 = *(_QWORD **)a2;
  *(_QWORD *)(a3 + 8) = *(_QWORD *)a2;
  if ( v6 != v4 )
  {
    if ( *(_DWORD *)(a2 + 8) )
    {
      *v6 = a3;
      if ( v6 == (_QWORD *)*v4 )
        *v4 = a3;
    }
    else
    {
      v6[2] = a3;
      if ( v6 == (_QWORD *)v4[2] )
        v4[2] = a3;
    }
    v7 = *(_QWORD *)(a3 + 8);
    for ( i = a3; ; v7 = *(_QWORD *)(i + 8) )
    {
      if ( *(_BYTE *)(v7 + 24) )
      {
        *(_BYTE *)(v4[1] + 24i64) = 1;
        return v3;
      }
      v9 = *(_QWORD *)(i + 8);
      v10 = *(__int64 **)(v9 + 8);
      v11 = *v10;
      if ( v9 == *v10 )
      {
        v11 = v10[2];
        if ( !*(_BYTE *)(v11 + 24) )
          goto LABEL_15;
        if ( i == *(_QWORD *)(v9 + 16) )
          std::_Tree_val<std::_Tree_simple_types<std::pair<std::wstring const,cxl::PropertyList::Property>>>::_Lrotate(a1, *(_QWORD *)(i + 8));
        *(_BYTE *)(*(_QWORD *)(i + 8) + 24i64) = 1;
        *(_BYTE *)(*(_QWORD *)(*(_QWORD *)(i + 8) + 8i64) + 24i64) = 0;
        std::_Tree_val<std::_Tree_simple_types<std::pair<std::wstring const,cxl::PropertyList::Property>>>::_Rrotate(a1, *(_QWORD *)(*(_QWORD *)(i + 8) + 8i64));
      }
      else
      {
        if ( !*(_BYTE *)(v11 + 24) )
        {
LABEL_15:
          *(_BYTE *)(v9 + 24) = 1;
          *(_BYTE *)(v11 + 24) = 1;
          *(_BYTE *)(*(_QWORD *)(*(_QWORD *)(i + 8) + 8i64) + 24i64) = 0;
          i = *(_QWORD *)(*(_QWORD *)(i + 8) + 8i64);
          continue;
        }
        if ( i == *(_QWORD *)v9 )
          std::_Tree_val<std::_Tree_simple_types<std::pair<std::wstring const,cxl::PropertyList::Property>>>::_Rrotate(a1, *(_QWORD *)(i + 8));
        *(_BYTE *)(*(_QWORD *)(i + 8) + 24i64) = 1;
        *(_BYTE *)(*(_QWORD *)(*(_QWORD *)(i + 8) + 8i64) + 24i64) = 0;
        std::_Tree_val<std::_Tree_simple_types<std::pair<std::wstring const,cxl::PropertyList::Property>>>::_Lrotate(a1, *(_QWORD *)(*(_QWORD *)(i + 8) + 8i64));
      }
    }
  }
  *v4 = a3;
  v4[1] = a3;
  v4[2] = a3;
  *(_BYTE *)(a3 + 24) = 1;
  return v3;
}


==========

FUNCTION: ?_Lrotate@?$_Tree_val@U?$_Tree_simple_types@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@@std@@@std@@QEAAXPEAU?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@2@@Z @ 0x180017B64
----------
_QWORD *__fastcall std::_Tree_val<std::_Tree_simple_types<std::pair<std::wstring const,cxl::PropertyList::Property>>>::_Lrotate(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(_QWORD **)(a2 + 16);
  *(_QWORD *)(a2 + 16) = *v2;
  if ( !*(_BYTE *)(*v2 + 25i64) )
    *(_QWORD *)(*v2 + 8i64) = a2;
  v2[1] = *(_QWORD *)(a2 + 8);
  result = *(_QWORD **)a1;
  if ( a2 == *(_QWORD *)(*(_QWORD *)a1 + 8i64) )
  {
    result[1] = v2;
  }
  else
  {
    result = *(_QWORD **)(a2 + 8);
    if ( a2 == *result )
      *result = v2;
    else
      result[2] = v2;
  }
  *v2 = a2;
  *(_QWORD *)(a2 + 8) = v2;
  return result;
}


==========

FUNCTION: ?_Reset_move@?$_Func_class@PEAUCLUSPROP_VALUE@@_K_K@std@@IEAAX$$QEAV12@@Z @ 0x180017BB4
----------
void __fastcall std::_Func_class<CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::_Reset_move(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = *(_QWORD *)(a2 + 56);
  if ( v4 )
  {
    if ( v4 == a2 )
    {
      *(_QWORD *)(a1 + 56) = (*(__int64 (__fastcall **)(__int64, __int64))(*(_QWORD *)v4 + 8i64))(v4, a1);
      std::function<void (CLUSPROP_ULARGE_INTEGER *)>::~function<void (CLUSPROP_ULARGE_INTEGER *)>(a2, v5);
    }
    else
    {
      *(_QWORD *)(a1 + 56) = v4;
      *(_QWORD *)(a2 + 56) = 0i64;
    }
  }
}


==========

FUNCTION: ?_Rrotate@?$_Tree_val@U?$_Tree_simple_types@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@@std@@@std@@QEAAXPEAU?$_Tree_node@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@PEAX@2@@Z @ 0x180017C0C
----------
_QWORD *__fastcall std::_Tree_val<std::_Tree_simple_types<std::pair<std::wstring const,cxl::PropertyList::Property>>>::_Rrotate(__int64 a1, _QWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *a2;
  *a2 = *(_QWORD *)(*a2 + 16i64);
  v3 = *(_QWORD *)(v2 + 16);
  if ( !*(_BYTE *)(v3 + 25) )
    *(_QWORD *)(v3 + 8) = a2;
  *(_QWORD *)(v2 + 8) = a2[1];
  result = *(_QWORD **)a1;
  if ( a2 == *(_QWORD **)(*(_QWORD *)a1 + 8i64) )
  {
    result[1] = v2;
  }
  else
  {
    result = (_QWORD *)a2[1];
    if ( a2 == (_QWORD *)result[2] )
      result[2] = v2;
    else
      *result = v2;
  }
  *(_QWORD *)(v2 + 16) = a2;
  a2[1] = v2;
  return result;
}


==========

FUNCTION: ?_Target_type@?$_Func_impl_no_alloc@V_lambda_061c9011cb9bb82bcf658bf6b36e3ac2_@@PEAUCLUSPROP_VALUE@@_K_K@std@@EEBAAEBVtype_info@@XZ @ 0x180017C60
----------
void *std::_Func_impl_no_alloc<_lambda_061c9011cb9bb82bcf658bf6b36e3ac2_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::_Target_type()
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return &_lambda_061c9011cb9bb82bcf658bf6b36e3ac2_ `RTTI Type Descriptor';
}


==========

FUNCTION: ?_Target_type@?$_Func_impl_no_alloc@V_lambda_13bf9359f506f7ffd00982af1d5f3a49_@@PEAUCLUSPROP_VALUE@@_K_K@std@@EEBAAEBVtype_info@@XZ @ 0x180017C80
----------
void *std::_Func_impl_no_alloc<_lambda_13bf9359f506f7ffd00982af1d5f3a49_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::_Target_type()
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return &_lambda_13bf9359f506f7ffd00982af1d5f3a49_ `RTTI Type Descriptor';
}


==========

FUNCTION: ?_Target_type@?$_Func_impl_no_alloc@V_lambda_37f697dbc2d2dfb7d8e4239a9e92d6a3_@@PEAUCLUSPROP_VALUE@@_K_K@std@@EEBAAEBVtype_info@@XZ @ 0x180017CA0
----------
void *std::_Func_impl_no_alloc<_lambda_37f697dbc2d2dfb7d8e4239a9e92d6a3_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::_Target_type()
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return &_lambda_37f697dbc2d2dfb7d8e4239a9e92d6a3_ `RTTI Type Descriptor';
}


==========

FUNCTION: ?_Target_type@?$_Func_impl_no_alloc@V_lambda_3b4ff329403cc3f130f9ace5ccfa8dfa_@@PEAUCLUSPROP_VALUE@@_K_K@std@@EEBAAEBVtype_info@@XZ @ 0x180017CB0
----------
void *std::_Func_impl_no_alloc<_lambda_3b4ff329403cc3f130f9ace5ccfa8dfa_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::_Target_type()
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return &_lambda_3b4ff329403cc3f130f9ace5ccfa8dfa_ `RTTI Type Descriptor';
}


==========

FUNCTION: ?_Target_type@?$_Func_impl_no_alloc@V_lambda_4ef0909d8e90c100aba481fabd6c4ca0_@@PEAUCLUSPROP_VALUE@@_K_K@std@@EEBAAEBVtype_info@@XZ @ 0x180017CC0
----------
void *std::_Func_impl_no_alloc<_lambda_4ef0909d8e90c100aba481fabd6c4ca0_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::_Target_type()
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return &_lambda_4ef0909d8e90c100aba481fabd6c4ca0_ `RTTI Type Descriptor';
}


==========

FUNCTION: ?_Target_type@?$_Func_impl_no_alloc@V_lambda_b2090a35b16344f94b4d1150e607dfd3_@@PEAUCLUSPROP_VALUE@@_K_K@std@@EEBAAEBVtype_info@@XZ @ 0x180017CF0
----------
void *std::_Func_impl_no_alloc<_lambda_b2090a35b16344f94b4d1150e607dfd3_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::_Target_type()
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return &_lambda_b2090a35b16344f94b4d1150e607dfd3_ `RTTI Type Descriptor';
}


==========

FUNCTION: ?_Target_type@?$_Func_impl_no_alloc@V_lambda_d97f77950fffadeda0a44d074e6b4f77_@@PEAUCLUSPROP_VALUE@@_K_K@std@@EEBAAEBVtype_info@@XZ @ 0x180017D20
----------
void *std::_Func_impl_no_alloc<_lambda_d97f77950fffadeda0a44d074e6b4f77_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::_Target_type()
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return &_lambda_d97f77950fffadeda0a44d074e6b4f77_ `RTTI Type Descriptor';
}


==========

FUNCTION: ?_Target_type@?$_Func_impl_no_alloc@V_lambda_e03fcb33fc3f30c21ab44e5b90795fb4_@@PEAUCLUSPROP_VALUE@@_K_K@std@@EEBAAEBVtype_info@@XZ @ 0x180017D40
----------
void *std::_Func_impl_no_alloc<_lambda_e03fcb33fc3f30c21ab44e5b90795fb4_,CLUSPROP_VALUE *,unsigned __int64,unsigned __int64>::_Target_type()
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return &_lambda_e03fcb33fc3f30c21ab44e5b90795fb4_ `RTTI Type Descriptor';
}


==========

FUNCTION: ?erase@?$vector@EV?$allocator@E@std@@@std@@QEAA?AV?$_Vector_iterator@V?$_Vector_val@U?$_Simple_types@E@std@@@std@@@2@V?$_Vector_const_iterator@V?$_Vector_val@U?$_Simple_types@E@std@@@std@@@2@0@Z @ 0x180017F04
----------
_QWORD *__fastcall std::vector<unsigned char>::erase(__int64 a1, _QWORD *a2, char *a3, char *a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a3 != a4 )
  {
    v7 = *(_QWORD *)(a1 + 8) - (_QWORD)a4;
    memmove_0(a3, a4, v7);
    *(_QWORD *)(a1 + 8) = &a3[v7];
  }
  result = a2;
  *a2 = a3;
  return result;
}


==========

FUNCTION: ?find@?$_Tree@V?$_Tmap_traits@V?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@UCaseInsensitive_LessThan@5@V?$allocator@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@@2@$0A@@std@@@std@@QEAA?AV?$_Tree_iterator@V?$_Tree_val@U?$_Tree_simple_types@U?$pair@$$CBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@UProperty@PropertyList@cxl@@@std@@@std@@@std@@@2@AEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@2@@Z @ 0x180017F68
----------
__int64 *__fastcall std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::find(__int64 *a1, __int64 *a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a2 = std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::_Find<std::wstring>(a1, a3);
  return a2;
}


==========

FUNCTION: ?insert@?$vector@EV?$allocator@E@std@@@std@@QEAA?AV?$_Vector_iterator@V?$_Vector_val@U?$_Simple_types@E@std@@@std@@@2@V?$_Vector_const_iterator@V?$_Vector_val@U?$_Simple_types@E@std@@@std@@@2@_KAEBE@Z @ 0x180018C40
----------
_QWORD *__fastcall std::vector<unsigned char>::insert(void **a1, _QWORD *a2, _BYTE *a3, size_t a4, unsigned __int8 *a5)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v28 = a2;
  v8 = *a1;
  Src = v8;
  v9 = a1[1];
  Size = a3 - (_BYTE *)v8;
  v10 = (_BYTE *)a1[2] - v9;
  if ( a4 == 1 && a3 == v9 )
  {
    v11 = 1;
  }
  else
  {
    v11 = 0;
    if ( !a4 )
      goto LABEL_17;
  }
  if ( a4 <= v10 )
  {
    v18 = *a5;
    if ( v11 )
    {
      *v9 = v18;
      a1[1] = (char *)a1[1] + 1;
    }
    else
    {
      v26[0] = *a5;
      v19 = v9 - a3;
      if ( a4 <= v9 - a3 )
      {
        memmove_0(v9, &v9[-a4], a4);
        a1[1] = &v9[a4];
        memmove_0(&v9[-(v19 - a4)], a3, v19 - a4);
        std::fill_n<unsigned char *,unsigned __int64,unsigned char>(a3, a4, v26);
      }
      else
      {
        v20 = a4 - v19;
        v21 = v18;
        memset_0(v9, v18, a4 - v19);
        v22 = &v9[v20];
        a1[1] = v22;
        memmove_0(v22, a3, v19);
        a1[1] = &v22[v19];
        memset_0(a3, v21, v19);
      }
    }
  }
  else
  {
    v12 = v9 - (_BYTE *)v8;
    if ( a4 > 0x7FFFFFFFFFFFFFFFi64 - (v9 - (_BYTE *)v8) )
      std::vector<wchar_t>::_Xlength();
    v25 = v12 + a4;
    v27 = std::vector<unsigned char>::_Calculate_growth(a1, v12 + a4);
    v14 = std::_Allocate_at_least_helper<std::allocator<unsigned char>>(v13, (size_t *)&v27);
    v24 = (size_t)v14 + Size;
    memset_0((char *)v14 + Size, *a5, a4);
    v15 = Src;
    v16 = v14;
    if ( v11 )
    {
      v17 = v12;
    }
    else
    {
      memmove_0(v14, Src, Size);
      v16 = (void *)(a4 + v24);
      v17 = v9 - a3;
      v15 = a3;
    }
    memmove_0(v16, v15, v17);
    std::vector<unsigned char>::_Change_array((__int64)a1, (__int64)v14, v25, v27);
  }
LABEL_17:
  result = v28;
  *v28 = (char *)*a1 + Size;
  return result;
}


==========

FUNCTION: ?push_back@?$vector@V?$tuple@_KUProperty@PropertyList@cxl@@_N@std@@V?$allocator@V?$tuple@_KUProperty@PropertyList@cxl@@_N@std@@@2@@std@@QEAAX$$QEAV?$tuple@_KUProperty@PropertyList@cxl@@_N@2@@Z @ 0x18001917C
----------
char *__fastcall std::vector<std::tuple<unsigned __int64,cxl::PropertyList::Property,bool>>::push_back(__int64 *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = (void *)a1[1];
  if ( v3 == (void *)a1[2] )
    return std::vector<std::tuple<unsigned __int64,cxl::PropertyList::Property,bool>>::_Emplace_reallocate<std::tuple<unsigned __int64,cxl::PropertyList::Property,bool>>(a1, v3, a2);
  result = (char *)std::_Construct_in_place<std::tuple<unsigned __int64,cxl::PropertyList::Property,bool>,std::tuple<unsigned __int64,cxl::PropertyList::Property,bool>>((__int64)v3, a2);
  *(_QWORD *)(v5 + 8) += 32i64;
  return result;
}


==========

FUNCTION: ??$_Construct@$00PEBD@?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@AEAAXQEBD_K@Z @ 0x18001A01C
----------
void *__fastcall std::string::_Construct<1,char const *>(_QWORD *a1, const void *a2, size_t a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a3 > 0x7FFFFFFFFFFFFFFFi64 )
    std::_Xlen_string();
  a1[3] = 15i64;
  if ( a3 > 0xF )
  {
    v7 = std::string::_Calculate_growth((__int64)a1, a3);
    v8 = std::_Allocate<16,std::_Default_allocate_traits,0>(v7 + 1);
    *a1 = v8;
    a1[2] = a3;
    a1[3] = v7;
    v9 = v8;
    result = memcpy_0(v8, a2, a3);
    *((_BYTE *)v9 + a3) = 0;
  }
  else
  {
    a1[2] = a3;
    result = memcpy_0(a1, a2, a3);
    *((_BYTE *)a1 + a3) = 0;
  }
  return result;
}


==========

FUNCTION: ??$_Reallocate_grow_by@V_lambda_19662282d61fd793232134d409f2e084_@@$$V@?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEAAAEAV01@_KV_lambda_19662282d61fd793232134d409f2e084_@@@Z @ 0x18001A0C8
----------
void *__fastcall std::wstring::_Reallocate_grow_by<_lambda_19662282d61fd793232134d409f2e084_,>(void *Src, unsigned __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *((_QWORD *)Src + 2);
  if ( 0x7FFFFFFFFFFFFFFEi64 - v2 < a2 )
    std::_Xlen_string();
  v4 = *((_QWORD *)Src + 3);
  v5 = v2 + a2;
  v6 = std::wstring::_Calculate_growth((__int64)Src, v2 + a2);
  if ( (unsigned __int64)(v6 + 1) > 0x7FFFFFFFFFFFFFFFi64 )
    std::_Throw_bad_array_new_length();
  v7 = std::_Allocate<16,std::_Default_allocate_traits,0>(2 * (v6 + 1));
  *((_QWORD *)Src + 2) = v5;
  v8 = v7;
  *((_QWORD *)Src + 3) = v6;
  v9 = 2 * v2 + 2;
  if ( v4 <= 7 )
  {
    memcpy_0(v7, Src, v9);
  }
  else
  {
    v10 = *(void **)Src;
    memcpy_0(v7, *(const void **)Src, v9);
    std::_Deallocate<16,0>(v10, 2 * v4 + 2);
  }
  *(_QWORD *)Src = v8;
  return Src;
}


==========

FUNCTION: ??$_Reallocate_grow_by@V_lambda_1dfe18491bcca09701d8ccb01d0b0af4_@@PEB_W_K@?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEAAAEAV01@_KV_lambda_1dfe18491bcca09701d8ccb01d0b0af4_@@PEB_W_K@Z @ 0x18001A1A0
----------
void *__fastcall std::wstring::_Reallocate_grow_by<_lambda_1dfe18491bcca09701d8ccb01d0b0af4_,wchar_t const *,unsigned __int64>(void *Src, unsigned __int64 a2, __int64 a3, const void *a4, __int64 a5)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v5 = *((_QWORD *)Src + 2);
  if ( 0x7FFFFFFFFFFFFFFEi64 - v5 < a2 )
    std::_Xlen_string();
  v8 = *((_QWORD *)Src + 3);
  v9 = v5 + a2;
  v10 = std::wstring::_Calculate_growth((__int64)Src, v5 + a2);
  if ( (unsigned __int64)(v10 + 1) > 0x7FFFFFFFFFFFFFFFi64 )
    std::_Throw_bad_array_new_length();
  v11 = std::_Allocate<16,std::_Default_allocate_traits,0>(2 * (v10 + 1));
  v12 = 2 * v5;
  *((_QWORD *)Src + 2) = v9;
  v13 = v11;
  *((_QWORD *)Src + 3) = v10;
  v14 = (char *)v11 + 2 * v5;
  v15 = 2 * a5;
  v16 = v5 + a5;
  if ( v8 <= 7 )
  {
    memcpy_0(v11, Src, v12);
    memcpy_0(v14, a4, v15);
    *((_WORD *)v13 + v16) = 0;
  }
  else
  {
    v17 = *(void **)Src;
    memcpy_0(v11, *(const void **)Src, v12);
    memcpy_0(v14, a4, v15);
    *((_WORD *)v13 + v16) = 0;
    std::_Deallocate<16,0>(v17, 2 * v8 + 2);
  }
  *(_QWORD *)Src = v13;
  return Src;
}


==========

FUNCTION: ??$_Reallocate_grow_by@V_lambda_25953b27f3c43b57ba59f021c7f225c5_@@_W@?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEAAAEAV01@_KV_lambda_25953b27f3c43b57ba59f021c7f225c5_@@_W@Z @ 0x18001A2BC
----------
void *__fastcall std::wstring::_Reallocate_grow_by<_lambda_25953b27f3c43b57ba59f021c7f225c5_,wchar_t>(void *Src, __int64 a2, __int64 a3, __int16 a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = *((_QWORD *)Src + 2);
  if ( v4 == 0x7FFFFFFFFFFFFFFEi64 )
    std::_Xlen_string();
  v7 = *((_QWORD *)Src + 3);
  v8 = v4 + 1;
  v9 = std::wstring::_Calculate_growth((__int64)Src, v4 + 1);
  if ( (unsigned __int64)(v9 + 1) > 0x7FFFFFFFFFFFFFFFi64 )
    std::_Throw_bad_array_new_length();
  v10 = std::_Allocate<16,std::_Default_allocate_traits,0>(2 * (v9 + 1));
  v11 = 2 * v4;
  *((_QWORD *)Src + 2) = v8;
  *((_QWORD *)Src + 3) = v9;
  v12 = v10;
  if ( v7 <= 7 )
  {
    memcpy_0(v10, Src, v11);
    *(_WORD *)((char *)v12 + v11) = a4;
    *(_WORD *)((char *)v12 + v11 + 2) = 0;
  }
  else
  {
    v13 = *(void **)Src;
    memcpy_0(v10, *(const void **)Src, v11);
    *(_WORD *)((char *)v12 + v11) = a4;
    *(_WORD *)((char *)v12 + v11 + 2) = 0;
    std::_Deallocate<16,0>(v13, 2 * v7 + 2);
  }
  *(_QWORD *)Src = v12;
  return Src;
}


==========

FUNCTION: ??$_Reallocate_grow_by@V_lambda_a3050a43f3157934f354774ab3dd2e02_@@_K_W@?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEAAAEAV01@_KV_lambda_a3050a43f3157934f354774ab3dd2e02_@@_K_W@Z @ 0x18001A3C0
----------
_QWORD *__fastcall std::wstring::_Reallocate_grow_by<_lambda_a3050a43f3157934f354774ab3dd2e02_,unsigned __int64,wchar_t>(_QWORD *a1, unsigned __int64 a2, __int64 a3, __int64 a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = a1[2];
  if ( 0x7FFFFFFFFFFFFFFEi64 - v4 < a2 )
    std::_Xlen_string();
  v7 = a1[3];
  v8 = v4 + a2;
  v9 = std::wstring::_Calculate_growth((__int64)a1, v4 + a2);
  if ( (unsigned __int64)(v9 + 1) > 0x7FFFFFFFFFFFFFFFi64 )
    std::_Throw_bad_array_new_length();
  v10 = std::_Allocate<16,std::_Default_allocate_traits,0>(2 * (v9 + 1));
  a1[2] = v8;
  v12 = v10;
  a1[3] = v9;
  if ( v7 <= 7 )
  {
    _lambda_a3050a43f3157934f354774ab3dd2e02_::operator()(v11, v10, a1, v4, a4);
  }
  else
  {
    v13 = (void *)*a1;
    _lambda_a3050a43f3157934f354774ab3dd2e02_::operator()(v11, v10, *a1, v4, a4);
    std::_Deallocate<16,0>(v13, 2 * v7 + 2);
  }
  *a1 = v12;
  return a1;
}


==========

FUNCTION: ??0?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@QEAA@QEBD@Z @ 0x18001A5F0
----------
_QWORD *__fastcall std::string::string(_QWORD *a1, _BYTE *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_OWORD *)a1 = 0i64;
  a1[2] = 0i64;
  a1[3] = 0i64;
  v3 = -1i64;
  do
    ++v3;
  while ( a2[v3] );
  std::string::_Construct<1,char const *>(a1, a2, v3);
  return a1;
}


==========

FUNCTION: ?append@?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@QEAAAEAV12@QEB_W_K@Z @ 0x18001AA80
----------
_QWORD *__fastcall std::wstring::append(_QWORD *a1, const void *a2, unsigned __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = a1[2];
  if ( a3 > a1[3] - v3 )
    return std::wstring::_Reallocate_grow_by<_lambda_1dfe18491bcca09701d8ccb01d0b0af4_,wchar_t const *,unsigned __int64>(a1, a3, a3, a2, a3);
  v5 = v3 + a3;
  a1[2] = v3 + a3;
  v7 = std::_String_val<std::_Simple_types<wchar_t>>::_Myptr((__int64)a1);
  memmove_0((char *)v7 + 2 * v8, v9, 2 * v6);
  *((_WORD *)v7 + v5) = 0;
  return a1;
}


==========

FUNCTION: ?push_back@?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@QEAAX_W@Z @ 0x18001AAF0
----------
_QWORD *__fastcall std::wstring::push_back(_QWORD *a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = a1[2];
  if ( v2 >= a1[3] )
    return std::wstring::_Reallocate_grow_by<_lambda_25953b27f3c43b57ba59f021c7f225c5_,wchar_t>(a1, a2, v2, a2);
  a1[2] = v2 + 1;
  result = std::_String_val<std::_Simple_types<wchar_t>>::_Myptr((__int64)a1);
  *((_WORD *)result + v5) = v4;
  *((_WORD *)result + v5 + 1) = 0;
  return result;
}


==========

FUNCTION: ?reserve@?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@QEAAX_K@Z @ 0x18001AB34
----------
void __fastcall std::wstring::reserve(_QWORD *a1, unsigned __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = a1[2];
  if ( v2 <= a2 && a1[3] != a2 )
  {
    if ( a1[3] >= a2 )
    {
      if ( a2 <= 7 && std::_String_val<std::_Simple_types<wchar_t>>::_Large_mode_engaged((__int64)a1) )
      {
        v5 = *v4;
        memcpy_0(v4, *v4, 2 * v2 + 2);
        std::wstring::_Deallocate_for_capacity(v6, v5, a1[3]);
        a1[3] = 7i64;
      }
    }
    else
    {
      std::wstring::_Reallocate_grow_by<_lambda_19662282d61fd793232134d409f2e084_,>(a1, a2 - v2);
      a1[2] = v2;
    }
  }
}


==========

FUNCTION: ?resize@?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@QEAAX_K_W@Z @ 0x18001ABB4
----------
_QWORD *__fastcall std::wstring::resize(_QWORD *a1, unsigned __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = a1[2];
  if ( a2 > v2 )
  {
    v7 = a2 - v2;
    if ( v7 > a1[3] - v2 )
    {
      result = std::wstring::_Reallocate_grow_by<_lambda_a3050a43f3157934f354774ab3dd2e02_,unsigned __int64,wchar_t>(a1, v7, 0i64, v7);
    }
    else
    {
      a1[2] = a2;
      result = std::_String_val<std::_Simple_types<wchar_t>>::_Myptr((__int64)a1);
      v10 = result;
      v12 = (_WORD *)result + v11;
      if ( v8 )
      {
        result = 0i64;
        for ( i = v8; i; --i )
          *v12++ = 0;
      }
      *((_WORD *)v10 + v9) = 0;
    }
  }
  else
  {
    result = std::_String_val<std::_Simple_types<wchar_t>>::_Myptr((__int64)a1);
    a1[2] = v6;
    *((_WORD *)result + v6) = 0;
  }
  return result;
}


==========

FUNCTION: ?load@?$_Atomic_storage@_K$07@std@@QEBA_KW4memory_order@2@@Z @ 0x18001AC2C
----------
__int64 __fastcall std::_Atomic_storage<unsigned __int64,8>::load(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return *(_QWORD *)a1;
}


==========

FUNCTION: ??$_Traits_equal@U?$char_traits@D@std@@@std@@YA_NQEBD_K01@Z @ 0x18001B180
----------
char __fastcall std::_Traits_equal<std::char_traits<char>>(const void *a1, size_t a2, __int64 a3, __int64 a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = 0;
  if ( a2 == a4 && !memcmp_0(a1, "raw", a2) )
    v4 = 1;
  return v4;
}


==========

FUNCTION: ??$_Reallocate_for@V_lambda_66f57f934f28d61049862f64df852ff0_@@PEBD@?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@AEAAAEAV01@_KV_lambda_66f57f934f28d61049862f64df852ff0_@@PEBD@Z @ 0x18001C734
----------
__int64 __fastcall std::string::_Reallocate_for<_lambda_66f57f934f28d61049862f64df852ff0_,char const *>(__int64 a1, size_t a2, __int64 a3, const void *a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a2 > 0x7FFFFFFFFFFFFFFFi64 )
    std::_Xlen_string();
  v7 = *(_QWORD *)(a1 + 24);
  v8 = std::string::_Calculate_growth(a1, a2);
  v9 = std::_Allocate<16,std::_Default_allocate_traits,0>(v8 + 1);
  *(_QWORD *)(a1 + 16) = a2;
  *(_QWORD *)(a1 + 24) = v8;
  v10 = v9;
  memcpy_0(v9, a4, a2);
  *((_BYTE *)v10 + a2) = 0;
  if ( v7 > 0xF )
    std::_Deallocate<16,0>(*(void **)a1, v7 + 1);
  *(_QWORD *)a1 = v10;
  return a1;
}


==========

FUNCTION: ?assign@?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@QEAAAEAV12@QEBD_K@Z @ 0x18001C7F8
----------
__int64 __fastcall std::string::assign(__int64 a1, const void *a2, size_t a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a3 > *(_QWORD *)(a1 + 24) )
    return std::string::_Reallocate_for<_lambda_66f57f934f28d61049862f64df852ff0_,char const *>(a1, a3, a3, a2);
  v5 = std::_String_val<std::_Simple_types<char>>::_Myptr(a1);
  *(_QWORD *)(a1 + 16) = v6;
  v7 = v5;
  memmove_0(v5, v8, v6);
  result = a1;
  *((_BYTE *)v7 + a3) = 0;
  return result;
}


==========

FUNCTION: ??0exception_ptr@std@@QEAA@AEBV01@@Z @ 0x18001D2A0
----------
std::exception_ptr *__fastcall std::exception_ptr::exception_ptr(std::exception_ptr *this, const struct std::exception_ptr *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)this = 0i64;
  *((_QWORD *)this + 1) = 0i64;
  __ExceptionPtrCopy(this, a2);
  return this;
}


==========

FUNCTION: ??1exception_ptr@std@@QEAA@XZ @ 0x18001D2D0
----------
// attributes: thunk
void __fastcall std::exception_ptr::~exception_ptr(std::exception_ptr *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  __ExceptionPtrDestroy(this);
}


==========

FUNCTION: ?default_error_condition@error_category@std@@UEBA?AVerror_condition@2@H@Z @ 0x18001D520
----------
__int64 __fastcall std::error_category::default_error_condition(__int64 a1, __int64 a2, int a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_DWORD *)a2 = a3;
  result = a2;
  *(_QWORD *)(a2 + 8) = a1;
  return result;
}


==========

FUNCTION: ?equivalent@error_category@std@@UEBA_NAEBVerror_code@2@H@Z @ 0x18001D540
----------
bool __fastcall std::error_category::equivalent(std::error_category *this, const struct std::error_code *a2, int a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return *((_QWORD *)this + 1) == *(_QWORD *)(*((_QWORD *)a2 + 1) + 8i64) && *(_DWORD *)a2 == a3;
}


==========

FUNCTION: ?equivalent@error_category@std@@UEBA_NHAEBVerror_condition@2@@Z @ 0x18001D570
----------
bool __fastcall std::error_category::equivalent(std::error_category *this, unsigned int a2, const struct std::error_condition *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = (*(__int64 (__fastcall **)(std::error_category *, char *, _QWORD))(*(_QWORD *)this + 24i64))(this, v6, a2);
  return *(_QWORD *)(*(_QWORD *)(v4 + 8) + 8i64) == *(_QWORD *)(*((_QWORD *)a3 + 1) + 8i64) && *(_DWORD *)v4 == *(_DWORD *)a3;
}


==========

FUNCTION: ?rethrow_exception@std@@YAXVexception_ptr@1@@Z @ 0x18001D5B8
----------
void __fastcall std::rethrow_exception(const void *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  __ExceptionPtrRethrow(a1);
  __debugbreak();
  JUMPOUT(0x18001D5CEi64);
}


==========

FUNCTION: ??0bad_alloc@std@@QEAA@XZ @ 0x18001D5D4
----------
std::bad_alloc *__fastcall std::bad_alloc::bad_alloc(std::bad_alloc *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *((_QWORD *)this + 2) = 0i64;
  *((_QWORD *)this + 1) = "bad allocation";
  *(_QWORD *)this = &std::bad_alloc::`vftable';
  return this;
}


==========

FUNCTION: ??$_Construct_in_place@V?$vector@EV?$allocator@E@std@@@std@@$$V@std@@YAXAEAV?$vector@EV?$allocator@E@std@@@0@@Z @ 0x18001D5FC
----------
__int64 __fastcall std::_Construct_in_place<std::vector<unsigned char>,>(_QWORD *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  result = 0i64;
  *a1 = 0i64;
  a1[1] = 0i64;
  a1[2] = 0i64;
  return result;
}


==========

FUNCTION: ??$_Set_ptr_rep_and_enable_shared@V?$vector@EV?$allocator@E@std@@@std@@@?$shared_ptr@$$CBV?$vector@EV?$allocator@E@std@@@std@@@std@@AEAAXQEAV?$vector@EV?$allocator@E@std@@@1@QEAV_Ref_count_base@1@@Z @ 0x18001D614
----------
void __fastcall std::shared_ptr<std::vector<unsigned char> const>::_Set_ptr_rep_and_enable_shared<std::vector<unsigned char>>(__int64 a1, __int64 a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)&cxl::EmptyByteVectorPtr = a2;
  *((_QWORD *)&cxl::EmptyByteVectorPtr + 1) = a3;
}


==========

FUNCTION: ??_E?$_Ref_count_obj2@$$CBV?$vector@EV?$allocator@E@std@@@std@@@std@@UEAAPEAXI@Z @ 0x18001D630
----------
_QWORD *__fastcall std::_Ref_count_obj2<std::vector<unsigned char> const>::`vector deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &std::_Ref_count_obj2<std::vector<unsigned char> const>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ?_Destroy@?$_Ref_count_obj2@$$CBV?$vector@EV?$allocator@E@std@@@std@@@std@@EEAAXXZ @ 0x18001D670
----------
__int64 std::_Ref_count_obj2<std::vector<unsigned char> const>::_Destroy()
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return std::_Destroy_in_place<std::vector<unsigned char>>();
}


==========

FUNCTION: ??0system_error@std@@QEAA@HAEBVerror_category@1@PEBD@Z @ 0x18001FFEC
----------
std::system_error *__fastcall std::system_error::system_error(std::system_error *this, int a2, const struct std::error_category *a3, const char *a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  DWORD1(v8) = HIDWORD(this);
  std::string::string(v9, a4);
  LODWORD(v8) = a2;
  *((_QWORD *)&v8 + 1) = a3;
  std::_System_error::_System_error((__int64)this, &v8, (__int64)v9);
  std::string::_Tidy_deallocate((__int64)v9);
  *(_QWORD *)this = &std::system_error::`vftable';
  return this;
}


==========

FUNCTION: ??4?$shared_ptr@$$CBVMiSession@mi@@@std@@QEAAAEAV01@$$QEAV01@@Z @ 0x180020154
----------
_QWORD *__fastcall std::shared_ptr<mi::MiSession const>::operator=(_QWORD *a1, __int64 *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *a2;
  *a2 = 0i64;
  v4 = a2[1];
  a2[1] = 0i64;
  *a1 = v2;
  v5 = (std::_Ref_count_base *)a1[1];
  a1[1] = v4;
  if ( v5 )
    std::_Ref_count_base::_Decref(v5);
  return a1;
}


==========

FUNCTION: ??$_Construct_in_place@VMiApplication@mi@@Uprivate_make_shared_tag@12@@std@@YAXAEAVMiApplication@mi@@$$QEAUprivate_make_shared_tag@12@@Z @ 0x1800206C4
----------
__int64 __fastcall std::_Construct_in_place<mi::MiApplication,mi::MiApplication::private_make_shared_tag>(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)a1 = 0i64;
  result = 0i64;
  *(_QWORD *)(a1 + 8) = 0i64;
  *(_OWORD *)(a1 + 16) = 0i64;
  *(_QWORD *)(a1 + 32) = 0i64;
  *(_QWORD *)(a1 + 40) = 0i64;
  *(_WORD *)(a1 + 48) = 1;
  return result;
}


==========

FUNCTION: ??$_Construct_in_place@VMiSession@mi@@V?$shared_ptr@VMiApplication@mi@@@std@@AEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@4@@std@@YAXAEAVMiSession@mi@@$$QEAV?$shared_ptr@VMiApplication@mi@@@0@AEBV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@0@@Z @ 0x1800206EC
----------
_QWORD *__fastcall std::_Construct_in_place<mi::MiSession,std::shared_ptr<mi::MiApplication>,std::wstring const &>(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *(_QWORD *)a1 = 0i64;
  *(_QWORD *)(a1 + 8) = 0i64;
  std::shared_ptr<mi::MiApplication>::shared_ptr<mi::MiApplication>(a1 + 16);
  *(_OWORD *)(a1 + 32) = 0i64;
  *(_QWORD *)(a1 + 48) = 0i64;
  *(_QWORD *)(a1 + 56) = 0i64;
  result = std::wstring::wstring((_QWORD *)(a1 + 64), v2);
  *(_BYTE *)(a1 + 96) = 1;
  *(_QWORD *)(a1 + 160) = 0i64;
  *(_QWORD *)(a1 + 224) = 0i64;
  *(_QWORD *)(a1 + 288) = 0i64;
  return result;
}


==========

FUNCTION: ??$_Set_ptr_rep_and_enable_shared@VMiSession@mi@@@?$shared_ptr@VMiSession@mi@@@std@@AEAAXQEAVMiSession@mi@@QEAV_Ref_count_base@1@@Z @ 0x180020754
----------
void __fastcall std::shared_ptr<mi::MiSession>::_Set_ptr_rep_and_enable_shared<mi::MiSession>(_QWORD *a1, _QWORD *a2, __int64 a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = a2;
  a1[1] = a3;
  if ( a2 )
  {
    v3 = a2[1];
    if ( !v3 || !*(_DWORD *)(v3 + 8) )
    {
      if ( a3 )
        _InterlockedIncrement((volatile signed __int32 *)(a3 + 8));
      v4 = (volatile signed __int32 *)a1[1];
      v5 = 0i64;
      v6 = 0i64;
      if ( v4 )
      {
        v5 = a2;
        v6 = a1[1];
        _InterlockedIncrement(v4 + 3);
      }
      *a2 = v5;
      v7 = (std::_Ref_count_base *)a2[1];
      a2[1] = v6;
      if ( v7 )
        std::_Ref_count_base::_Decwref(v7);
      if ( v4 )
        std::_Ref_count_base::_Decref((std::_Ref_count_base *)v4);
    }
  }
}


==========

FUNCTION: ??$make_shared@VMiApplication@mi@@Uprivate_make_shared_tag@12@@std@@YA?AV?$shared_ptr@VMiApplication@mi@@@0@$$QEAUprivate_make_shared_tag@MiApplication@mi@@@Z @ 0x1800207C8
----------
_QWORD *__fastcall std::make_shared<mi::MiApplication,mi::MiApplication::private_make_shared_tag>(_QWORD *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v10 = operator new(0x48ui64);
  *(_OWORD *)v10 = 0i64;
  v10[2] = 1;
  v10[3] = 1;
  *(_QWORD *)v10 = &std::_Ref_count_obj2<mi::MiApplication>::`vftable';
  std::_Construct_in_place<mi::MiApplication,mi::MiApplication::private_make_shared_tag>((__int64)(v10 + 4));
  *a1 = v2;
  a1[1] = v3;
  if ( !v2 )
    return a1;
  v5 = v2[1];
  if ( v5 && *(_DWORD *)(v5 + 8) )
    return a1;
  _InterlockedAdd((volatile signed __int32 *)(v3 + 8), v4);
  v6 = (volatile signed __int32 *)a1[1];
  if ( v6 )
  {
    *(_QWORD *)&v7 = v2;
    *((_QWORD *)&v7 + 1) = a1[1];
    _InterlockedAdd(v6 + 3, v4);
  }
  else
  {
    v7 = 0i64;
  }
  *v2 = v7;
  *(_QWORD *)&v7 = v2[1];
  v2[1] = *((_QWORD *)&v7 + 1);
  if ( (_QWORD)v7 )
    std::_Ref_count_base::_Decwref(v8);
  if ( v6 )
    std::_Ref_count_base::_Decref((std::_Ref_count_base *)v6);
  return a1;
}


==========

FUNCTION: ??0bad_weak_ptr@std@@QEAA@AEBV01@@Z @ 0x180020894
----------
std::bad_weak_ptr *__fastcall std::bad_weak_ptr::bad_weak_ptr(std::bad_weak_ptr *this, const struct std::bad_weak_ptr *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  std::exception::exception(this, a2);
  *(_QWORD *)this = &std::bad_weak_ptr::`vftable';
  return this;
}


==========

FUNCTION: ??_E?$_Ref_count_obj2@VMiApplication@mi@@@std@@UEAAPEAXI@Z @ 0x180020900
----------
_QWORD *__fastcall std::_Ref_count_obj2<mi::MiApplication>::`vector deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &std::_Ref_count_obj2<mi::MiApplication>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ??_G?$_Ref_count_obj2@VMiSession@mi@@@std@@UEAAPEAXI@Z @ 0x180020940
----------
_QWORD *__fastcall std::_Ref_count_obj2<mi::MiSession>::`scalar deleting destructor'(_QWORD *a1, char a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = &std::_Ref_count_obj2<mi::MiSession>::`vftable';
  if ( (a2 & 1) != 0 )
    operator delete(a1);
  return a1;
}


==========

FUNCTION: ?_Destroy@?$_Ref_count_obj2@VMiApplication@mi@@@std@@EEAAXXZ @ 0x180020C70
----------
void __fastcall std::_Ref_count_obj2<mi::MiApplication>::_Destroy(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  mi::MiApplication::~MiApplication((mi::MiApplication *)(a1 + 16));
}


==========

FUNCTION: ?_Destroy@?$_Ref_count_obj2@VMiSession@mi@@@std@@EEAAXXZ @ 0x180020C80
----------
void __fastcall std::_Ref_count_obj2<mi::MiSession>::_Destroy(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  mi::MiSession::~MiSession((mi::MiSession *)(a1 + 16));
}


==========

FUNCTION: ?_Incref_nz@_Ref_count_base@std@@QEAA_NXZ @ 0x180020C90
----------
char __fastcall std::_Ref_count_base::_Incref_nz(std::_Ref_count_base *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v1 = *((_DWORD *)this + 2);
  while ( v1 )
  {
    v2 = v1;
    v1 = _InterlockedCompareExchange((volatile signed __int32 *)this + 2, v1 + 1, v1);
    if ( v2 == v1 )
      return 1;
  }
  return 0;
}


==========

FUNCTION: ?_Throw_bad_weak_ptr@std@@YAXXZ @ 0x180020CB0
----------
void __noreturn std::_Throw_bad_weak_ptr(void)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  pExceptionObject = &std::bad_weak_ptr::`vftable';
  v1 = 0i64;
  CxxThrowException_0(&pExceptionObject, (_ThrowInfo *)&TI2_AVbad_weak_ptr_std__);
}


==========

FUNCTION: ?shared_from_this@?$enable_shared_from_this@VMiSession@mi@@@std@@QEBA?AV?$shared_ptr@$$CBVMiSession@mi@@@2@XZ @ 0x180020CE0
----------
_QWORD *__fastcall std::enable_shared_from_this<mi::MiSession>::shared_from_this(__int64 a1, _QWORD *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *(std::_Ref_count_base **)(a1 + 8);
  *a2 = 0i64;
  a2[1] = 0i64;
  if ( !v2 || !std::_Ref_count_base::_Incref_nz(v2) )
    std::_Throw_bad_weak_ptr();
  *v3 = *v4;
  v3[1] = v4[1];
  return v3;
}


==========

FUNCTION: ?what@bad_weak_ptr@std@@UEBAPEBDXZ @ 0x180020D30
----------
const char *__fastcall std::bad_weak_ptr::what(std::bad_weak_ptr *this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return "bad_weak_ptr";
}


==========

FUNCTION: ??$?4U?$default_delete@UMiOperationArgs@MiAsyncOperation@mi@@@std@@$0A@@?$unique_ptr@UMiOperationArgs@MiAsyncOperation@mi@@U?$default_delete@UMiOperationArgs@MiAsyncOperation@mi@@@std@@@std@@QEAAAEAV01@$$QEAV01@@Z @ 0x180020D40
----------
__int64 *__fastcall std::unique_ptr<mi::MiAsyncOperation::MiOperationArgs>::operator=<std::default_delete<mi::MiAsyncOperation::MiOperationArgs>,0>(__int64 *a1, __int64 *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = *a2;
  *a2 = 0i64;
  v4 = *a1;
  *a1 = v2;
  if ( v4 )
    std::default_delete<mi::MiAsyncOperation::MiOperationArgs>::operator()();
  return a1;
}


==========

FUNCTION: ??1?$unique_ptr@UMiOperationArgs@MiAsyncOperation@mi@@U?$default_delete@UMiOperationArgs@MiAsyncOperation@mi@@@std@@@std@@QEAA@XZ @ 0x1800210AC
----------
__int64 __fastcall std::unique_ptr<mi::MiAsyncOperation::MiOperationArgs>::~unique_ptr<mi::MiAsyncOperation::MiOperationArgs>(_QWORD *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( *a1 )
    result = std::default_delete<mi::MiAsyncOperation::MiOperationArgs>::operator()();
  return result;
}


==========

FUNCTION: ??R?$default_delete@UMiOperationArgs@MiAsyncOperation@mi@@@std@@QEBAXPEAUMiOperationArgs@MiAsyncOperation@mi@@@Z @ 0x180021454
----------
void __fastcall std::default_delete<mi::MiAsyncOperation::MiOperationArgs>::operator()(__int64 a1, mi::MiAsyncOperation::MiOperationArgs *a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a2 )
  {
    mi::MiAsyncOperation::MiOperationArgs::~MiOperationArgs(a2);
    operator delete(a2);
  }
}


==========

FUNCTION: ?max@?$duration@_JU?$ratio@$00$0DOI@@std@@@chrono@std@@SA?AV123@XZ @ 0x180021780
----------
_QWORD *__fastcall std::chrono::duration<__int64,std::ratio<1,1000>>::max(_QWORD *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  *a1 = 0x7FFFFFFFFFFFFFFFi64;
  return a1;
}


==========

FUNCTION: ??1?$enable_shared_from_this@VMiSession@mi@@@std@@IEAA@XZ @ 0x180021890
----------
void __fastcall std::enable_shared_from_this<mi::MiSession>::~enable_shared_from_this<mi::MiSession>(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v1 = *(std::_Ref_count_base **)(a1 + 8);
  if ( v1 )
    std::_Ref_count_base::_Decwref(v1);
}


==========

