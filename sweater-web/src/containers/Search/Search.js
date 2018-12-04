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
        minWidth: '275px'
        , marginBottom: '16px'
    }
});

class Search extends Component {

    state = {
        torrents: [
            {
                infoHash: 'derp'
                , magnetUri: 'magnet:?xt=urn:btih:608c9e4070398f02757492cf3817783ee93fa32d&dn=Hackers+%281995%29+720p+BrRip+x264+-+YIFY&tr=udp%3A%2F%2Ftracker.openbittorrent.com%3A80&tr=udp%3A%2F%2Ftracker.publicbt.com%3A80&tr=udp%3A%2F%2Ftracker.ccc.de%3A80'
                , name: 'Hackers 1995'
                , size: '500 MiB'
                , seeders: 666
                , leechers: 12
                , uploadedOn: '09-14 2016'
            },
            {
                infoHash: 'derp'
                , magnetUri: 'magnet:?xt=urn:btih:608c9e4070398f02757492cf3817783ee93fa32d&dn=Hackers+%281995%29+720p+BrRip+x264+-+YIFY&tr=udp%3A%2F%2Ftracker.openbittorrent.com%3A80&tr=udp%3A%2F%2Ftracker.publicbt.com%3A80&tr=udp%3A%2F%2Ftracker.ccc.de%3A80'
                , name: 'Hackers 1995'
                , size: '1.5 GiB'
                , seeders: 666
                , leechers: 12
                , uploadedOn: '07-23 16:07'
            }
        ]
    };

    render = () => {
        const {classes} = this.props;

        return (
            <>
                <div className={classes.search}>
                    {this.state.torrents.map((t, i) => (
                        <TorrentCard
                            key={i}
                            className={classes.torrentCard}
                            name={t.name}
                            magnetUri={t.magnetUri}
                            uploadedOn={t.uploadedOn}
                            size={t.size}
                            seeders={t.seeders}
                            leechers={t.seeders}/>
                    ))}
                </div>
            </>
        );
    }
}


export default withStyles(styles)(Search);
