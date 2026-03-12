Source file: Microsoft.FailoverClusters.FrameworkSupport
Generated time: 20260130_193637
==========

FUNCTION: ??0?$ListHandle@VPropertyList@cxl@@@PropList@@QEAA@AEBV01@@Z @ 0x18000B274
----------
__int64 __fastcall PropList::ListHandle<cxl::PropertyList>::ListHandle<cxl::PropertyList>(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = a1 + 8;
  cxl::PropertyList::PropertyList((struct cxl::PropertyList *)(a1 + 8));
  cxl::PropertyList::property_iterator::property_iterator((struct cxl::PropertyList::property_iterator *)(a1 + 48));
  cxl::PropertyList::property_iterator::property_iterator((struct cxl::PropertyList::property_iterator *)(a1 + 192));
  *(_DWORD *)a1 = *(_DWORD *)a2;
  if ( v4 != a2 + 8 )
  {
    std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::clear((_QWORD *)v4);
    std::_Tree<std::_Tmap_traits<std::wstring,cxl::PropertyList::Property,cxl::CaseInsensitive_LessThan,std::allocator<std::pair<std::wstring const,cxl::PropertyList::Property>>,0>>::_Copy<0>((_QWORD *)v4, (_QWORD *)(a2 + 8));
  }
  std::vector<unsigned char>::operator=((void **)(v4 + 16), a2 + 24);
  cxl::PropertyList::property_iterator::operator=(a1 + 48, a2 + 48);
  cxl::PropertyList::property_iterator::operator=(a1 + 192, a2 + 192);
  *(_BYTE *)(a1 + 336) = *(_BYTE *)(a2 + 336);
  return a1;
}


==========

FUNCTION: ??0?$ListHandle@VValueList@cxl@@@PropList@@QEAA@AEBV01@@Z @ 0x18000B338
----------
__int64 __fastcall PropList::ListHandle<cxl::ValueList>::ListHandle<cxl::ValueList>(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  cxl::ValueList::ValueList_void_((struct cxl::ValueList *)(a1 + 8));
  cxl::ValueList::value_iterator::value_iterator((struct cxl::ValueList::value_iterator *)(a1 + 96), v4);
  cxl::ValueList::value_iterator::value_iterator((struct cxl::ValueList::value_iterator *)(a1 + 144), v5);
  *(_DWORD *)a1 = *(_DWORD *)a2;
  std::vector<unsigned char>::operator=((void **)(a1 + 8), a2 + 8);
  std::function<CLUSPROP_VALUE * (unsigned __int64,unsigned __int64)>::operator=(a1 + 32, a2 + 32);
  *(_QWORD *)(a1 + 96) = *(_QWORD *)(a2 + 96);
  std::shared_ptr<mi::MiApplication>::operator=((__int64 *)(a1 + 104));
  *(_QWORD *)(a1 + 120) = *(_QWORD *)(a2 + 120);
  *(_QWORD *)(a1 + 128) = *(_QWORD *)(a2 + 128);
  *(_QWORD *)(a1 + 144) = *(_QWORD *)(a2 + 144);
  std::shared_ptr<mi::MiApplication>::operator=((__int64 *)(a1 + 152));
  *(_QWORD *)(a1 + 168) = *(_QWORD *)(a2 + 168);
  *(_QWORD *)(a1 + 176) = *(_QWORD *)(a2 + 176);
  *(_BYTE *)(a1 + 192) = *(_BYTE *)(a2 + 192);
  return a1;
}


==========

FUNCTION: ??1?$AutoList@VPropertyList@cxl@@@PropList@@QEAA@XZ @ 0x18000B688
----------
void *__fastcall PropList::AutoList<cxl::PropertyList>::~AutoList<cxl::PropertyList>(void **a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v1 = *a1;
  if ( v1 )
    result = PropList::ListHandle<cxl::PropertyList>::`scalar deleting destructor'(v1);
  return result;
}


==========

FUNCTION: ??1?$AutoList@VValueList@cxl@@@PropList@@QEAA@XZ @ 0x18000B6A8
----------
void *__fastcall PropList::AutoList<cxl::ValueList>::~AutoList<cxl::ValueList>(void **a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v1 = *a1;
  if ( v1 )
    result = PropList::ListHandle<cxl::ValueList>::`scalar deleting destructor'(v1);
  return result;
}


==========

FUNCTION: ??1?$ListHandle@VPropertyList@cxl@@@PropList@@QEAA@XZ @ 0x18000B6C8
----------
void __fastcall PropList::ListHandle<cxl::PropertyList>::~ListHandle<cxl::PropertyList>(__int64 a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)(a1 + 192));
  cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)(a1 + 48));
  cxl::PropertyList::~PropertyList((struct cxl::PropertyList *)(a1 + 8));
}


==========

FUNCTION: ??1?$ListHandle@VValueList@cxl@@@PropList@@QEAA@XZ @ 0x18000B6FC
----------
void __fastcall PropList::ListHandle<cxl::ValueList>::~ListHandle<cxl::ValueList>(_QWORD *a1)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = (std::_Ref_count_base *)a1[20];
  if ( v2 )
    std::_Ref_count_base::_Decref(v2);
  v3 = (std::_Ref_count_base *)a1[14];
  if ( v3 )
    std::_Ref_count_base::_Decref(v3);
  cxl::ValueList::~ValueList((struct cxl::ValueList *)(a1 + 1));
}


==========

FUNCTION: ??E?$ListHandle@VPropertyList@cxl@@@PropList@@QEAA?AV01@H@Z @ 0x18000B9D4
----------
__int64 __fastcall PropList::ListHandle<cxl::PropertyList>::operator++(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  PropList::ListHandle<cxl::PropertyList>::ListHandle<cxl::PropertyList>(a2, a1);
  PropList::ListHandle<cxl::PropertyList>::next(a1);
  return a2;
}


==========

FUNCTION: ??F?$ListHandle@VPropertyList@cxl@@@PropList@@QEAA?AV01@H@Z @ 0x18000BA20
----------
__int64 __fastcall PropList::ListHandle<cxl::PropertyList>::operator--(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  PropList::ListHandle<cxl::PropertyList>::ListHandle<cxl::PropertyList>(a2, a1);
  cxl::PropertyList::property_iterator::operator=(a1 + 48, a1 + 192);
  return a2;
}


==========

FUNCTION: ??_G?$ListHandle@VPropertyList@cxl@@@PropList@@QEAAPEAXI@Z @ 0x18000C9F4
----------
void *__fastcall PropList::ListHandle<cxl::PropertyList>::`scalar deleting destructor'(void *Block)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  PropList::ListHandle<cxl::PropertyList>::~ListHandle<cxl::PropertyList>((__int64)Block);
  operator delete(Block);
  return Block;
}


==========

FUNCTION: ??_G?$ListHandle@VValueList@cxl@@@PropList@@QEAAPEAXI@Z @ 0x18000CA20
----------
void *__fastcall PropList::ListHandle<cxl::ValueList>::`scalar deleting destructor'(void *Block)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  PropList::ListHandle<cxl::ValueList>::~ListHandle<cxl::ValueList>(Block);
  operator delete(Block);
  return Block;
}


==========

FUNCTION: ?end@?$ListHandle@VPropertyList@cxl@@@PropList@@QEBA?AVproperty_iterator@PropertyList@cxl@@XZ @ 0x18000D690
----------
__int64 __fastcall PropList::ListHandle<cxl::PropertyList>::end(__int64 a1, __int64 a2)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  cxl::PropertyList::end((struct cxl::PropertyList *)(a1 + 8), (struct cxl::PropertyList::property_iterator *)a2);
  return a2;
}


==========

FUNCTION: ?next@?$ListHandle@VPropertyList@cxl@@@PropList@@AEAAXXZ @ 0x18000DDC4
----------
void __fastcall PropList::ListHandle<cxl::PropertyList>::next(__int64 this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !*(_BYTE *)(this + 336) )
  {
    v2 = (struct cxl::PropertyList::property_iterator *)(this + 48);
    cxl::PropertyList::property_iterator::operator=(this + 192, this + 48);
    v3 = cxl::PropertyList::end((struct cxl::PropertyList *)(this + 8), (struct cxl::PropertyList::property_iterator *)v10);
    v4 = cxl::PropertyList::property_iterator::operator==((__int64)v2, (__int64)v3);
    cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v10);
    if ( v4 )
    {
      v8 = *(_QWORD *)(this + 8);
      v9 = *(_QWORD *)v8;
      cxl::PropertyList::property_iterator::property_iterator((struct cxl::PropertyList::property_iterator *)v10, (__int64)&v9, (__int64)&v8, (struct cxl::PropertyList *)(this + 8), v7);
      cxl::PropertyList::property_iterator::operator=((__int64)v2, (__int64)v10);
      cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v10);
    }
    else
    {
      cxl::PropertyList::property_iterator::next_item(v2);
    }
    v5 = cxl::PropertyList::end((struct cxl::PropertyList *)(this + 8), (struct cxl::PropertyList::property_iterator *)v10);
    v6 = cxl::PropertyList::property_iterator::operator==((__int64)v2, (__int64)v5);
    cxl::PropertyList::property_iterator::~property_iterator((struct cxl::PropertyList::property_iterator *)v10);
    if ( v6 )
      *(_BYTE *)(this + 336) = 1;
  }
}


==========

FUNCTION: ?next@?$ListHandle@VValueList@cxl@@@PropList@@AEAAXXZ @ 0x18000DEE8
----------
void __fastcall PropList::ListHandle<cxl::ValueList>::next(__int64 this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !*(_BYTE *)(this + 192) )
  {
    v2 = (__int64 *)(this + 96);
    *(_QWORD *)(this + 144) = *(_QWORD *)(this + 96);
    std::shared_ptr<mi::MiApplication>::operator=((__int64 *)(this + 152));
    *(_QWORD *)(this + 168) = v2[3];
    *(_QWORD *)(this + 176) = v2[4];
    v3 = cxl::ValueList::end(this + 8, (struct cxl::ValueList::value_iterator *)v11);
    v4 = *v2;
    v5 = *(_QWORD *)v3;
    if ( v12 )
      std::_Ref_count_base::_Decref(v12);
    if ( v4 == v5 )
    {
      v6 = cxl::ValueList::begin(this + 8, (struct cxl::ValueList::value_iterator *)v11);
      *v2 = *(_QWORD *)v6;
      std::shared_ptr<mi::MiApplication>::operator=(v2 + 1);
      v2[3] = *((_QWORD *)v6 + 3);
      v7 = v12;
      v2[4] = *((_QWORD *)v6 + 4);
      if ( v7 )
        std::_Ref_count_base::_Decref(v7);
    }
    else
    {
      cxl::ValueList::value_iterator::next_value((struct cxl::ValueList::value_iterator *)v2);
    }
    v8 = cxl::ValueList::end(this + 8, (struct cxl::ValueList::value_iterator *)v11);
    v9 = *v2;
    v10 = *(_QWORD *)v8;
    if ( v12 )
      std::_Ref_count_base::_Decref(v12);
    if ( v9 == v10 )
      *(_BYTE *)(this + 192) = 1;
  }
}


==========

FUNCTION: ?reset@?$ListHandle@VValueList@cxl@@@PropList@@QEAAXXZ @ 0x18000E034
----------
void __fastcall PropList::ListHandle<cxl::ValueList>::reset(__int64 this)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v2 = cxl::ValueList::end(this + 8, (struct cxl::ValueList::value_iterator *)v4);
  *(_QWORD *)(this + 96) = *(_QWORD *)v2;
  std::shared_ptr<mi::MiApplication>::operator=((__int64 *)(this + 104));
  *(_QWORD *)(this + 120) = *((_QWORD *)v2 + 3);
  v3 = v5;
  *(_QWORD *)(this + 128) = *((_QWORD *)v2 + 4);
  if ( v3 )
    std::_Ref_count_base::_Decref(v3);
  *(_QWORD *)(this + 144) = *(_QWORD *)(this + 96);
  std::shared_ptr<mi::MiApplication>::operator=((__int64 *)(this + 152));
  *(_QWORD *)(this + 168) = *(_QWORD *)(this + 120);
  *(_QWORD *)(this + 176) = *(_QWORD *)(this + 128);
  *(_BYTE *)(this + 192) = 0;
}


==========

