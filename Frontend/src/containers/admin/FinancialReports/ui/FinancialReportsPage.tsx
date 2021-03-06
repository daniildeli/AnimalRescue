
import React from 'react';
import { Tabs } from 'antd';
import '../style/FinancialReportsPage.scss';
import { IFinancialReport, deleteFinancialReporDocument, addFinancialReporDocument } from '../../../../api/financialReport';
import { ReactComponent as Pdf } from '../../../../img/pdf.svg';
import moment from 'moment';
import { TI18n } from '../../../../i18n';
import { Button, ButtonTypes } from '../../../../components/Button';
import { DatePicker } from 'antd';
import { AdminMenu } from '../../AdminMenu';

const { TabPane } = Tabs;
const { RangePicker } = DatePicker;

interface IState {
 [key:string]: any;
}
interface IPropTypes {
  financeReports: IFinancialReport[];
  fetchFinancialReport: () => {};
};
export class FinancialReportsPage extends React.Component<IPropTypes, IState>{
  constructor(props: any) {
    super(props);
    this.state = {
      file: '',
      title: '',
      body: '',
      date: '',
    };
  }
  componentDidMount() {
    this.props.fetchFinancialReport()
  }

  handleSubmit(e: any) {
    e.preventDefault();
    const { file, title, body, date } = this.state;
    if (!file) {
      return;
    }

    let localDate = moment(date).format("L");
    const formData = new FormData();
    formData.append('title', title);
    formData.append('body', body);
    formData.append('date', localDate);
    formData.append('file', file, file.name);
    addFinancialReporDocument(formData).then(resp => {
      this.props.fetchFinancialReport();
    });
  }

  async uploadFile(e: any) {
    e.preventDefault();
    await this.setState({ file: e.target.files[0] });
  }

  handleFileInfo(field: string, e: any) {
    switch (field) {
      case 'title':
      case 'body':
        this.setState({ [field]: e.target.value });
        break;
      case 'date':
        this.setState({ [field]: e.toDate() });
        break;
    }
  }

  async deleteReport(id: string, title: string): Promise<any> {
    await deleteFinancialReporDocument(id)
    .then(resp=>{
      console.log(resp);
      if (resp.status === 200) {
        this.props.fetchFinancialReport();
      }
    })
    .catch((err) => {
      console.log(err);
    })
  }

  render() {
    return (
      <div className='boxAdmin'>
        <AdminMenu 
          selectedKey={'reports'}
          openKeys={['sub2', 'sub1']}
        />
        <main>
          <div className="container">
            <section className="section-reports">
              <header>
                <h3>Фінансові звіти</h3>
              </header>
              <div className="page-content">
                <Tabs defaultActiveKey="1">
                  <TabPane tab="Укр" key="1">
                     <div className="inner-top section-margin">
                       <form action="#">
                         <p>
                           <label>Заголовок</label>
                           <input className="input-h2" type="text"/>
                         </p>
                         <p>
                           <label>Передмова</label>
                           <textarea name="page-description"/>
                         </p>
                         <p>
                           <Button
                               styleType={ButtonTypes.BlueSmall}>
                             Зберегти зміни
                           </Button>
                         </p>
                       </form>
                     </div>
                    <section className="section-new-report">
                      <h4> Завантажити новий звiт</h4>
                      <form onSubmit={(e) => this.handleSubmit(e)} >
                        <div className='field'>
                          <label>Виберіть документ у pdf форматі</label><br/>
                          <input
                              id="downloadReport"
                              className="custom-file-input"
                              type="file"
                              onChange={(e) => this.uploadFile(e)} />
                          <label htmlFor="downloadReport"><span/></label>
                        </div>
                        {/*<div className='field'>*/}
                        {/*  <label>Додати ім'я:</label><br/>*/}
                        {/*  <input className="fileInput"*/}
                        {/*         type="text"*/}
                        {/*         placeholder=''*/}
                        {/*         onChange={(e) => this.handleFileInfo('title', e)} />*/}
                        {/*</div>*/}
                        {/*<div className='field'>*/}
                        {/*  <label>Додати опис:</label><br/>*/}
                        {/*  <input className="fileInput"*/}
                        {/*         type="text"*/}
                        {/*         placeholder=''*/}
                        {/*         onChange={(e) => this.handleFileInfo('body', e)} />*/}
                        {/*</div>*/}
                        <div className='field'>
                          <label>Виберіть період звіту</label><br/>
                          <RangePicker
                              placeholder={["Початок", "Кінець"]}
                              // defaultValue={moment('YYYY-MM-DD')}
                              // onChange={(e) => this.handleFileInfo('date', e)}
                          />
                        </div>
                      </form>
                      <Button
                          onClick={(e) => this.handleSubmit(e)}
                          styleType={ButtonTypes.BlueOutlineSmall}>
                        <TI18n keyStr="addReport" />
                      </Button>
                    </section>
                    <section className="section-all-reports">
                      <h4>Звiти за всi роки</h4>
                      {this.props.financeReports.length > 0 &&
                      <div className="inner-all-reports">
                        <Tabs defaultActiveKey="1" tabPosition='top'>
                          {this.props.financeReports.map((report: IFinancialReport, i: number) => (
                              <TabPane tab={report.date} key={report.date + 1} >
                                <form action="#">
                                  <p>
                                    <label>Заголовок</label>
                                    <input className="input-h2" type="text"/>
                                  </p>
                                  <p>
                                    <label>Передмова</label>
                                    <textarea name="page-description"/>
                                  </p>
                                  <p>
                                    <Button
                                        styleType={ButtonTypes.BlueSmall}>
                                      Зберегти зміни
                                    </Button>
                                  </p>
                                </form>
                                <span className="title-uploaded-reports">Завантажені документи</span>
                                <div className="list-reports">
                                  {report.reports.map((list) => (
                                      <p key={list.fileId} className='report' >
                                        <i className="icon-pdf"/><a href="#" className="title-item"><TI18n keyStr={`dateText${moment(list.date).month()}`} /> <span className='year-report'>{moment(list.date).year()}</span></a>
                                        <button onClick={() => { this.deleteReport(list.fileId, list.title) }} className='delete icon-close' />
                                      </p>
                                  ))}
                                </div>
                              </TabPane>
                          ))}
                        </Tabs>
                      </div>
                      }
                    </section>
                  </TabPane>
                  <TabPane tab="Eng" key="2">
                    Content of Tab Pane 2
                  </TabPane>
                  <TabPane tab="Deu" key="3">
                    Content of Tab Pane 3
                  </TabPane>
                  <TabPane tab="Рус" key="4">
                    Content of Tab Pane 3
                  </TabPane>
                </Tabs>
              </div>
            </section>
          </div>
        </main>
      </div>
    );
  }
}