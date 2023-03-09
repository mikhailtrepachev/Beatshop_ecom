import React, { Component, useEffect, useState } from 'react';
import axios from "axios";
import { List, Typography } from "antd";

const { Title } = Typography;

export class Home extends Component {
    constructor(props) {
        super(props);
        this.state = {
            beats: []
        };
    }
    static displayName = Home.name;

    //make get() request after each loading
    componentDidMount() {
        this.axiosInstace.get('/get_beats_on_main_page')
            .then(response => {
                const beatsData = response.data;
                this.setState({ beats: beatsData});
            })
            .catch(error => {
                console.error(error);
            })
    }

    axiosInstace = axios.create({
      baseURL: '/api',
      withCredentials: true,
  });

  render() {
      const { beats } = this.state;
    return (
    <div>
        <Title level={2}>Beats</Title>
        <List
            itemLayout="horizontal"
            dataSource={beats}
            renderItem={beat => (
                <List.Item>
                    <List.Item.Meta
                        title = {<a href="#">{beat.name}</a>}
                        description={`${beat.description}`}
                    />
                </List.Item>
            )}
            />
    </div>
    );
  }
}

export default Home;
