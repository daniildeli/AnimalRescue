import React from 'react';
import { ITag, ITagValue } from '../../../../api/tags';
import { ELocales } from '../../../../i18n/store/state';
import { TI18n } from '../../../../i18n';
import { NavLink } from 'react-router-dom';
import './style.scss';

interface IPropTypes {
  category: string;
  tags: Array<ITag>;
}

export const TagsCategoryItem: React.FC<IPropTypes> = ({category, tags}) => {
  const getTagName = (values: Array<ITagValue>, isLast: boolean, divider?: string): string => {
    const currentValue: ITagValue | undefined = values.find(value => value.lang.toLowerCase() === ELocales.ua.toLowerCase());

    return !!currentValue
      ? `${currentValue.value}${isLast
        ? ''
        : `${!!divider ? divider : ', '}`}`
      : '';
  }
  return (
      <div className="с-item">
          <div className="row">
              <div className="col col-category">{<TI18n keyStr={`${category}TagCategory`} default={category} locale={ELocales.ua}/>}</div>
              <div className="col col-value">
                  {tags.map((tag, index) => (
                      <span key={index}>{getTagName(tag.values, index === tags.length - 1)}</span>
                  ))}
              </div>
              <div className="col col-edit">
                  <NavLink to={`/admin/tags/${category.toLowerCase()}`}>
                      Edit
                  </NavLink>
              </div>
          </div>
      </div>
  )
}