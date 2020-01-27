import React from 'react';
import { IBlockLinkPropTypes, BlockLink } from '../../../../components/BlockLink';
import { BlogItem } from '../../../../components/BlogBlock/item';
import { IBlogItem } from '../../../../api/blog';
import '../styles/Blog.scss';
import { TI18n } from '../../../../i18n';

interface IPropTypes {
    title: string | React.ReactNode;
    data: IBlogItem[]
    link?: IBlockLinkPropTypes
    showLink: boolean
  }

  export const BlogBlock: React.FC<IPropTypes> = ({ title, link, data}) => {
    console.log('====+', { title, link, data});
    return (
      <div className="blog-block-container">
          <div className="content">
            <div className="column-container">
                <div className="left-block title">{title}</div> 
                <div className="right-block">
                    {!!link && !!link.href && <BlockLink {...link}/>}
                </div>  
            </div>
            <ul className="blog-list-holder">
              { 
                data.map((blog)=> <BlogItem key={blog.title}
                  image={blog.imageIds[0]}
                  text={<TI18n keyStr="blogBlockItemType" default="Блог" />}
                  title={blog.title}
                  id={blog.id ? blog.id : ''}
                />)
              }
            </ul>
            <div className="bottom-redirect">
                    {!!link && !!link.href && <BlockLink {...link}/>}
                </div> 
          </div>
      </div>
    )
  }