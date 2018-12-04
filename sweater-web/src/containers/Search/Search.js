import React, {Component} from 'react';
import TorrentCard from "./TorrentCard/TorrentCard";
import {withStyles} from '@material-ui/core/styles';


const styles = theme => ({
    search: {
        // margin: '16px 16px'
        padding: '16px'
        , maxWidth: '100%'
        , [theme.breakpoints.up('sm')]: {
            maxWidth: '700px'
            , margin: 'auto auto'
        },
    },
    torrentCard: {
        // margin: '16px auto'
        marginBottom: '16px'
    }
});

class Search extends Component {

    state = {
        torrents: [
            {
                infoHash: 'derp'
                , magnetUri: 'derp'
                , name: 'Hackers 1995'
                , size: '500 MiB'
                , seeders: 666
                , leechers: 12
                , uploadedOn: 'Today'
            },
            {
                infoHash: 'derp'
                , magnetUri: 'derp'
                , name: 'Hackers 1995'
                , size: '500 MiB'
                , seeders: 666
                , leechers: 12
                , uploadedOn: 'Today'
            }
        ]
    };

    render = () => {
        const {classes} = this.props;

        return (
            <>
                <div className={classes.search}>
                    {this.state.torrents.map(t => (
                        <TorrentCard className={classes.torrentCard}/>
                    ))}
                </div>
            </>
        );
    }
}


export default withStyles(styles)(Search);
