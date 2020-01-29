import React from "react";
import {TI18n} from "../../../../i18n";
import {Button, ButtonTypes} from "../../../../components/Button";
import {ReactComponent as HeartLogo} from '../../../../assets/header/heart.svg';
import {Link} from'react-router-dom';
import { store } from "../../../../store";
import { actionIsActivePopup } from "../../Home/store/actions";
import "../styles/menu.scss"
import { RULES_PAGE_LINK } from "../../Rules";
import { HELP_PAGE_LINKS } from "../../HowToHelp";

export const AppMenu: React.FC = () => {
    return (
        <div className="header-menu">
            <div className="item">
                <TI18n keyStr="headerMenuItem1" default="О службе"/>
                <ul className="dropdown">
                    <li><Link to="/about"><TI18n keyStr="headerMenuItem1Dropdown1" default="Про службу порятунку" /></Link></li>
                    <li><Link to={RULES_PAGE_LINK}><TI18n keyStr="headerMenuItem1Dropdown2" default="Правила работы с нами" /></Link></li>
                    <li><Link to="/financial-reports"><TI18n keyStr="headerMenuItem1Dropdown3" default="Финансовые отчеты"/></Link></li>
                </ul>
            </div>
            <div className="item">
                <TI18n keyStr="headerMenuItem2" default="Ищу друга"/>
                <ul className="dropdown">
                    <li><a href="pet-any"><TI18n keyStr="headerMenuItem2Dropdown1" default="Любого"/></a></li>
                    <li><a href="pet-dog"><TI18n keyStr="headerMenuItem2Dropdown2" default="Собачку"/></a></li>
                    <li><a href="pet-cat"><TI18n keyStr="headerMenuItem2Dropdown3" default="Котика"/></a></li>
                    <li><a href="pet-the-loss"><TI18n keyStr="headerMenuItem2Dropdown4" default="Потеряшку"/></a></li>
                </ul>
            </div>
            <div className='item'>
              <Link to={HELP_PAGE_LINKS.default}><TI18n keyStr="headerMenuItem3" default="Как я могу помочь?" /></Link>
              <ul className="dropdown">
                <li><Link to={HELP_PAGE_LINKS.finance}><TI18n keyStr="headerMenuItem3Dropdown1" default="Финансово" /></Link></li>
                <li><Link to={HELP_PAGE_LINKS.stuff}><TI18n keyStr="headerMenuItem3Dropdown2" default="Вещами" /></Link></li>
                <li><Link to={HELP_PAGE_LINKS.volunteering}><TI18n keyStr="headerMenuItem3Dropdown3" default="Волонтерством" /></Link></li>
              </ul>
            </div>
            <div className="item"><TI18n keyStr="blog" default="Блог"/></div>
            <Link className="item" to="/contacts"><TI18n keyStr="contacts" default="Контакты"/></Link>
            <div className="item heart"><HeartLogo/></div>
            <Button onClick={() => {store.dispatch(actionIsActivePopup(true))
            }}  styleType={ButtonTypes.Blue}>
                <TI18n keyStr="help" default="Помочь"/>
            </Button>
        </div>
    )
};
