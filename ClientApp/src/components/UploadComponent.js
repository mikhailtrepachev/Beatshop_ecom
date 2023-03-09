import { message, Button, Popconfirm, Space, 
        Upload, Input, Tooltip, Typography, Select, 
        Col, Row, Divider, Modal, Steps} from 'antd';
import { UploadOutlined, PlusOutlined } from '@ant-design/icons';
import React, { Component, useState } from 'react';
import axios from 'axios';
import {AuthorizeService} from "./api-authorization/AuthorizeService";

const validExtentions = ['.wav', '.mp3'];

const { Option } = Select;
const { Text } = Typography;

const gutterForRows = [16, 16];


const jsonString = localStorage.getItem('Beatshopuser:https://localhost:44404:Beatshop');
const jsonObject = JSON.parse(jsonString);
const token = jsonObject.access_token;
//axios set up
const axiosInstance = axios.create({
    baseURL: '/api',
    withCredentials: true,
    headers: {
      'Content-Type': 'multipart/form-data',
      'Authorization': `Bearer ${token}`,
    },
});


//TODO: upload image to the database or AWS
const getBase64 = (file) =>

  new Promise((resolve, reject) => {

    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => resolve(reader.result);
    reader.onerror = (error) => reject(error);
  });

//Uploading Image UI
const UploadImage = () => {

  const [previewOpen, setPreviewOpen] = useState(false);
  const [previewImage, setPreviewImage] = useState('');
  const [previewTitle, setPreviewTitle] = useState('');
  const [fileList, setFileList] = useState([]);

  const handleCancel = () => setPreviewOpen(false);

  const handlePreview = async (file) => {

    if(!file.url && !file.preview) {

      file.preview = await getBase64(file.originFileObj);
    }
    setPreviewImage(file.url || file.preview);
    setPreviewOpen(true);
    setPreviewTitle(file.name || file.url.substring(file.url.lastIndexOf('/') + 1))
  };


  const handleChange = ({ fileList: newFileList }) => setFileList(newFileList);

  //Upload image button
  const uploadButton = (
    <div>
      <PlusOutlined />
      <div style={{
        marginTop: 5,
      }}
      >
        Upload an Album
      </div>
    </div>
  );

  return(
    <div>
      <Upload
        action = "https://www.mocky.io/v2/5cc8019d300000980a055e76"
        listType = "picture-card"
        fileList = {fileList}
        onPreview = {handlePreview}
        onChange = {handleChange}
      >
          {fileList.length > 1 ? null : uploadButton}
      </Upload>
      <Modal open={previewOpen} title={previewTitle} footer={null} onCancel={handleCancel}>
        <img
          alt="example"
          style={{
            width: '100%',
          }}
          src={previewImage}
          />
      </Modal>
    </div>
  );
};

//Steps component
const StepCounting = () => {

  const [currentStep, setCurrentStep] = useState(0);

  const onChange = (value) => {
    setCurrentStep(value);
  };

  const description = ['1. File uploading', '2. Price settings', '3. User agreement'];

  return (
    <>
      <Steps
        current={currentStep}
        onChange={onChange}
        items={[
          {
            title: 'Step 1',
            description: description[0],
          },
          {
            title: 'Step 2',
            description: description[1],
          },
          {
            title: 'Step 3',
            description: description[2],
          },
        ]}
        />
    </>
  );
}

export class UploadComponent extends Component {

  state = {
    fileList: [],
    trackName: '',
    description: '',
    genre: '',
  };

  handleInputChange = (event) => {
    const { name, value } = event.target;
    this.setState({ [name]: value });
  }

  handleGenreChange = (value) => {
    this.setState({ genre: value });
  };

  handleUploadChange = ({ file, fileList }) => {

    //Check file extention
    const extention = file.name.substring(file.name.lastIndexOf('.')).toLowerCase();
    if (!validExtentions.includes(extention)) {
      message.error('Only .WAV and .MP3 are allowed!');

      //Remove file from the fileList
      const index = fileList.indexOf(file);
      fileList.splice(index, 1);
      this.setState({ fileList });
      return;
    }

    this.setState({ fileList });
  };

  //API function for sending data to the server side
  handleSubmit = async (event) => {
    event.preventDefault();
    const { fileList, trackName, description, genre } = this.state;
    const formData = new FormData();
    formData.append('trackName', trackName);
    formData.append('trackFile', fileList[0].originFileObj);
    formData.append('description', description);
    formData.append('genre', genre);

    try {
      const response = await axiosInstance.post('/beatupload', formData);
      console.log(response);

      this.setState({
        fileList: [],
        trackName: '',
        description: '',
        genre: ''
      });

      message.success('Beat has been uploaded!');
    }
    catch (error) {
      console.error(error);
      console.log(error.response);
      message.error('Error uploading beat!')
    }
  };

  render() {
    const { fileList, trackName, description, genre } = this.state;

    return (
      <>
      <Divider orientation="left">Beat's uploading</Divider>
        <Row gutter={gutterForRows}>
          <Col span={4} push={2}>
            <Text>Track name:</Text>
          </Col>
          <Col span={8}>
            <Input placeholder="Track name" name="trackName" value={trackName} onChange={this.handleInputChange} />
          </Col>
          <Col span={12}>
            <div style={{
              position: 'relative',
              left: '50%',
            }}
            >
              <UploadImage />
            </div>
          </Col>
        </Row>
        <br />
        <Row gutter={gutterForRows}>
          <Col span={12}>
              <Input.TextArea placeholder="Description" name="description" value={description} onChange={this.handleInputChange} />
          </Col>
        </Row>
        <br />
        <Row gutter={gutterForRows}>
          <Col span={8}>
            <Select placeholder="Select Genre" 
                    onChange={this.handleGenreChange} 
                    style={{ width: 120, }}
                    defaultValue="rnb"
                    options={[
                      {
                        value: 'rnb',
                        label: 'RnB',
                      },
                      {
                        value: 'hip-hop',
                        label: 'Hip-hop',
                      },
                      {
                        value: 'dnb',
                        label: 'Drum and Bass',
                      },
                    ]}
            />
          </Col>
          <Col span={8}>
            <Upload fileList={fileList}
                    name={trackName}
                    onChange={this.handleUploadChange}
                    beforeUpload={() => false}
            >
              <Button icon={<UploadOutlined />}>
                .WAV or .MP3 File
              </Button> 
            </Upload>
          </Col>
        </Row>
        <br />
        <Row gutter={gutterForRows}>
          <Col span={8}>
            <Button type="primary" onClick={this.handleSubmit}>
              Upload
            </Button>
          </Col>
        </Row>
        <br />
      <StepCounting />
      <br />
        </>
    );
  }
} 

export default UploadComponent;